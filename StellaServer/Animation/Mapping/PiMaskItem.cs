using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServer.Animation.Mapping
{
    /// <summary>
    /// Points to a pi and pixelIndex on that pi.
    /// </summary>
    public class PiMaskItem
    {
        /// <summary> The index of the pi </summary>
        public int PiIndex { get; }

        /// <summary> The index of the pixel on the pi </summary>
        public int PixelIndex { get; }

        public PiMaskItem(int piIndex, int pixelIndex)
        {
            PiIndex = piIndex;
            PixelIndex = pixelIndex;
        }
    }
}
