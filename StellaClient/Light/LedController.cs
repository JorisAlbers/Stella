using System;
using System.Collections.Generic;
using System.Text;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    /// <summary>
    ///  Controls the LedStrip.
    /// </summary>
    public class LedController : ILedController
    {
        private readonly ILEDStrip _ledStrip;

        public LedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
        }

        public void RenderFrame(FrameWithoutDelta frame)
        {
            for (int i = 0; i < frame.Count; i++)
            {
                _ledStrip.SetLEDColor(0, i, frame[i].Color);
            }
            _ledStrip.Render();
        }
    }
}
