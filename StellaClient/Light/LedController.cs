using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    /// <summary>
    ///  Controls the LedStrip.
    /// </summary>
    public class LedController : ILedController
    {
        private const long MINIMUM_TICKS_PER_FRAME = TimeSpan.TicksPerMillisecond * 50;
        private readonly ILEDStrip _ledStrip;
        private long _nextRenderAllowedAfter;
        private object lockObject = new object();

        public LedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
            _nextRenderAllowedAfter = DateTime.Now.Ticks;
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
                            _ledStrip.SetLEDColor(0, i, frame[i].Color);
                        }

                        _ledStrip.Render();
                        _nextRenderAllowedAfter = now.Ticks + MINIMUM_TICKS_PER_FRAME;
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
