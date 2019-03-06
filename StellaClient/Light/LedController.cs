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
        private Queue<Frame> _framesBuffer;
        private object _frameBufferLock = new object();
        private System.Timers.Timer _timer;
        private long _frameStart = -1;
        private Frame _nextFrame;
        private bool _isDisposed;
       

        public int FramesInBuffer {get{return _framesBuffer.Count;}}

        public LedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
            _framesBuffer = new Queue<Frame>();
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
            if(_nextFrame == null)
            {
                // CASE 1 , Prepare, Render, Prepare
                if(_framesBuffer.Count > 0)
                {
                    Frame firstFrame = _framesBuffer.Dequeue();
                    PrepareFrame(firstFrame);
                    _ledStrip.Render();
                    _frameStart = DateTime.Now.Ticks + (firstFrame.WaitMS * TimeSpan.TicksPerMillisecond);
                    if(_framesBuffer.Count > 0)
                    {
                        _nextFrame = _framesBuffer.Dequeue();
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
                _ledStrip.Render();
                _frameStart += _nextFrame.WaitMS * TimeSpan.TicksPerMillisecond;
                if(_framesBuffer.Count > 0)
                {
                    // Prepare next frame
                    _nextFrame = _framesBuffer.Dequeue();
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
            for(int i=0; i< frame.Count;i++)
            {
                PixelInstruction instruction = frame[i];
                _ledStrip.SetLEDColor(0,(int)instruction.Index,instruction.Color);
            }
        }

        /// <summary>
        /// Adds a frame to the queue to display.
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrame(Frame frame)
        {
            lock(_frameBufferLock)
            {
                _framesBuffer.Enqueue(frame);
            }
        }

        /// <summary>
        /// Adds multiple frames to the queue to display.
        /// </summary>
        /// <param name="frame"></param>
        public void AddFrames(IEnumerable<Frame> frames)
        {
            lock(_frameBufferLock)
            {
                foreach(Frame frame in frames)
                {
                    _framesBuffer.Enqueue(frame);
                }
            }
        }

        public void ClearFrameBuffer()
        {
            lock(_frameBufferLock)
            {
                _framesBuffer.Clear();
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}