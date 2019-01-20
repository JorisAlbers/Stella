using System.Drawing;

namespace rpi_ws281x
{
    /// <summary>
    /// Interface for ledstrips controlling objects
    /// </summary>
    public interface ILEDStrip
    {
        void Render();
        void SetLEDColor(int channelIndex, int ledID, Color color);
    }
}