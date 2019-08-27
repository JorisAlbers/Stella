using System;
using System.Threading;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClientLib.Light
{
    /// <summary>
    ///  Controls the LedStrip.
    /// </summary>
    public class LedController : ILedController
    {
        private readonly long _minimumTicksPerFrame;
        private readonly ILEDStrip _ledStrip;
        private long _nextRenderAllowedAfter;
        private object lockObject = new object();

        public LedController(ILEDStrip ledStrip, int minimumFrameRate)
        {
            _ledStrip = ledStrip;
            _nextRenderAllowedAfter = DateTime.Now.Ticks;
            _minimumTicksPerFrame = 1000 / minimumFrameRate * TimeSpan.TicksPerMillisecond;
        }

        public void RenderFrame(FrameWithoutDelta frame)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    DateTime now = DateTime.Now;
                    if (now.Ticks >= _nextRenderAllowedAfter)
                    {
                        for (int i = 0; i < frame.Count; i++)
                        {
                            PixelInstruction instruction = frame[i];

                            _ledStrip.SetLEDColor(0, i, System.Drawing.Color.FromArgb(instruction.R, instruction.G, instruction.B));
                        }

                        _ledStrip.Render();
                        _nextRenderAllowedAfter = now.Ticks + _minimumTicksPerFrame;
                    }
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
        }
    }
}
