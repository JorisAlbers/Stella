using System;
using System.Collections.Generic;
using System.Text;
using rpi_ws281x;
using StellaLib.Animation;

namespace StellaClient.Light
{
    /// <summary>
    ///  A led controller without a buffer
    /// </summary>
    public class BufferlessLedController
    {
        private readonly ILEDStrip _ledStrip;

        public BufferlessLedController(ILEDStrip ledStrip)
        {
            _ledStrip = ledStrip;
        }

        public void PrepareFrame(FrameWithoutDelta frame)
        {
            for (int i = 0; i < frame.Count; i++)
            {
                _ledStrip.SetLEDColor(0, i, frame[i].Color);
            }
        }

        public void Render()
        {
            _ledStrip.Render();
        }
    }
}
