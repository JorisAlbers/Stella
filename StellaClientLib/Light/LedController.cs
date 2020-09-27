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
                    for (int i = 0; i < frame.Count; i++)
                    {
                        PixelInstruction instruction = frame[i];

                        _ledStrip.SetLEDColor(0, i, System.Drawing.Color.FromArgb(instruction.R, instruction.G, instruction.B));
                    }

                    _ledStrip.Render();
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
        }
    }
}
