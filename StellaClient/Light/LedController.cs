using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    public class LedController : IDisposable
    {
        /// Time tests:
        /// 
        /// Technique                 |   Pixels   |  Time
        /// All pixels to a different |    300     |  9 at most
        /// color                     | 
        /// Moving dot                |    300     |  13 at most
        /// 
        private const int TIMER_LOOP_DURATION = 15; // in miliseconds
        private ILEDStrip _ledStrip;
        private System.Timers.Timer _timer;
        private bool _isDisposed;

        /// CURRENT fields
        private FrameSetMetadata _frameSetMetadata;
        private Queue<Frame> _frameBuffer;
        private readonly object _frameBufferLock = new object();
        private Frame _nextFrame;
        private long _frameStart = -1; //TODO use TimeStampRelative instead of waitMS. Can be implemented after FrameSet has been implemented.


        /// PENDING fields
        private FrameSetMetadata _pendingFrameSetMetadata;
        private Queue<Frame> _pendingFrameBuffer;

        public int FramesInBuffer
        {
            get
            {
                lock (_frameBufferLock)
                {
                    if (_frameBuffer == null)
                    {
                        return 0;
                    }
                    return _frameBuffer.Count;
                }
            }
        }

        public int FramesInPendingBuffer
        {
            get
            {
                lock (_frameBufferLock)
                {
                    if (_pendingFrameBuffer == null)
                    {
                        return 0;
                    }
                    return _pendingFrameBuffer.Count;
                }
            }
        }

        public LedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
            _frameBuffer = new Queue<Frame>();
            _timer = new System.Timers.Timer();
            _timer.Interval = TIMER_LOOP_DURATION;
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
        }

        public void Run()
        {
            _nextFrame = null;
            _timer.Enabled = true;
        }

       
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(_isDisposed)
            {
                _timer.Stop();
                return;
            }

            lock(_frameBufferLock)
            {
                Draw();
            }            
        }

        /// The flow is as follows:
        /// 
        /// Prepare frame 1             ||
        /// Start immediately           || Case 1
        /// Prepare frame 2             ||
        /// 
        /// Wait frame1 display time    ||
        /// Start frame 2               || Case 2  
        /// Prepare frame 3             ||
        /// 
        /// Wait frame 2 display time   ||
        /// Start frame 3               || Case 2
        /// Prepare frame 4             ||
        /// 
        /// ...
        private void Draw()
        {
            long now = DateTime.Now.Ticks;

            // First, check if the pending frameSet should be drawn
            if (_pendingFrameSetMetadata != null)
            {
                if (now >= _pendingFrameSetMetadata.TimeStamp.Ticks + TIMER_LOOP_DURATION)
                {
                    _frameSetMetadata = _pendingFrameSetMetadata;
                    _frameBuffer = _pendingFrameBuffer;
                    _frameStart = -1;
                    _nextFrame = null;
                    _pendingFrameSetMetadata = null;
                    _pendingFrameBuffer = null;
                }
            }
            
            // Then, check if we are in case 1
            if(_nextFrame == null)
            {
                if (_frameBuffer.Count > 0 && now >= _frameSetMetadata.TimeStamp.Ticks + TIMER_LOOP_DURATION)
                {
                    // CASE 1 , Prepare, Render, Prepare
                    // Frame 1
                    Frame firstFrame = _frameBuffer.Dequeue();
                    PrepareFrame(firstFrame);
                    // should render in this loop, wait for remaining time
                    while (DateTime.Now.Ticks < _frameSetMetadata.TimeStamp.Ticks)
                    {
                        Thread.Sleep(5); // TODO thread.sleep is unreliable and inefficient
                    }
                    Render();

                    // Frame 2
                    if(_frameBuffer.Count > 0)
                    {
                        _nextFrame = _frameBuffer.Dequeue();
                        _frameStart = _frameSetMetadata.TimeStamp.Ticks + _nextFrame.TimeStampRelative * TimeSpan.TicksPerMillisecond;
                        PrepareFrame(_nextFrame);
                    }

                }
            }
            else
            {
                // CASE 2, Render, Prepare
                if(DateTime.Now.Ticks < _frameStart + TIMER_LOOP_DURATION)
                {
                    // render will happen in other loop
                    return;
                }

                // should render in this loop, wait for remaing time
                while(DateTime.Now.Ticks < _frameStart)
                {
                    Thread.Sleep(5); // TODO thread.sleep is unreliable and inefficient
                }
                // Render the already loaded frame
                Render();
                if(_frameBuffer.Count > 0)
                {
                    // Prepare next frame
                    _nextFrame = _frameBuffer.Dequeue();
                    _frameStart = _frameSetMetadata.TimeStamp.Ticks + _nextFrame.TimeStampRelative * TimeSpan.TicksPerMillisecond;
                    PrepareFrame(_nextFrame);
                }
                else
                {
                    _nextFrame = null;
                }
            }
        }

        private void PrepareFrame(Frame frame)
        {
            //Console.Out.WriteLine($"LedController: preparing frame {frame.Index}. RTS : {frame.TimeStampRelative}");
            for(int i=0; i< frame.Count;i++)
            {
                PixelInstruction instruction = frame[i];
                _ledStrip.SetLEDColor(0,(int)instruction.Index,instruction.Color);
            }
        }

        private void Render()
        {
            //Console.Out.WriteLine($"Rendering frame.");
            _ledStrip.Render();
        }

        /// <summary>
        /// Adds a frame to the queue to display.
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrame(Frame frame)
        {
            lock(_frameBufferLock)
            {
                if (_frameSetMetadata == null && _pendingFrameSetMetadata == null)
                {
                    throw new Exception(
                        $"Failed to add frames as the frameSet was not prepared. Call {nameof(PrepareNextFrameSet)} before adding frames.");
                }
                if (_pendingFrameBuffer == null)
                {
                    _frameBuffer.Enqueue(frame);
                }
                else
                {
                    _pendingFrameBuffer.Enqueue(frame);
                }
            }
        }

        /// <summary>
        /// Adds multiple frames to the queue to display.
        /// </summary>
        /// <param name="frames"></param>
        public void AddFrames(IEnumerable<Frame> frames)
        {
            lock(_frameBufferLock)
            {
                if (_frameSetMetadata == null && _pendingFrameSetMetadata == null)
                {
                    throw new Exception(
                        $"Failed to add frames as the frameSet was not prepared. Call { nameof(PrepareNextFrameSet) } before adding frames.");
                }

                if (_pendingFrameBuffer == null)
                {
                    foreach (Frame frame in frames)
                    {
                        _frameBuffer.Enqueue(frame);
                    }
                }
                else
                {
                    foreach (Frame frame in frames)
                    {
                        _pendingFrameBuffer.Enqueue(frame);
                    }
                }
            }
        }

        /// <summary>
        /// Prepares the next frameset. This will make sure the current one is still playing whilst the next one gets loaded.
        /// </summary>
        /// <param name="metadata"></param>
        public void PrepareNextFrameSet(FrameSetMetadata metadata)
        {
            lock (_frameBufferLock)
            {
                _pendingFrameSetMetadata = metadata;
                _pendingFrameBuffer = new Queue<Frame>();
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}