using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    // TODO add IDispose implementation
    public class LedController
    {
        private ILEDStrip _ledStrip;
        private Queue<Frame> _framesBuffer;

        /// <summary>
        /// Frame rate in miliseconds
        /// </summary>
        private int _frameVisibleForMiliseconds = 1000;

        public int FramesInBuffer {get{return _framesBuffer.Count;}}

        public LedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
            _framesBuffer = new Queue<Frame>();
        }

        public async void Run()
        {
            Task task = new Task(()=>
            {
                long start = DateTime.Now.Ticks;
                while(true)
                {
                    Frame nextFrame = null;
                    lock(_framesBuffer)
                    {
                        if(_framesBuffer.Count > 0)
                        {
                            nextFrame = _framesBuffer.Dequeue();
                        }
                    }

                    int sleepForMilliseconds = 1000;
                    if(nextFrame != null)
                    {
                        // Display the frame
                        DrawFrame(nextFrame);
                        // Wait till frame rate passed
                        int elapsedMs = (int) ((start - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond);
                        sleepForMilliseconds = elapsedMs - _frameVisibleForMiliseconds;
                    }

                    // TODO | just waiting here is not efficient. We need to:
                    // TODO | 1. Check if the queue has changed
                    // TODO | 2. use a UNIX timestamp instead of framerate to keep in sync
                    Thread.Sleep(sleepForMilliseconds);
                }
            });
            // TODO check if explicit start is necessary task.Start();
            await task;
            Console.WriteLine("LedController has stopped working.");
        }

        private void DrawFrame(Frame frame)
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
            //TODO maybe add IEnumerable overload for the frame param to not keep requesting the lock for each item
            lock(_framesBuffer)
            {
                _framesBuffer.Enqueue(frame);
            }
        }
    }
}