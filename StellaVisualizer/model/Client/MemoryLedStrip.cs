using System;
using System.Drawing;
using rpi_ws281x;

namespace StellaVisualizer.Model.Client
{
    public class MemoryLedStrip : ILEDStrip
    {
        private Color[] _frame;

        public event EventHandler<Color[]> RenderRequested;

        public MemoryLedStrip(int numberOfPixels)
        {
            _frame = new Color[numberOfPixels];
        }

        public void Render()
        {
            var eventHandler = RenderRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this, _frame);
            }
        }

        public void SetLEDColor(int channelIndex, int ledID, Color color)
        {
            if (channelIndex != 0)
            {
                throw new NotImplementedException();
            }

            _frame[ledID] = color;
        }
    }
}
