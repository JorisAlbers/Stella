using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StellaServerLib.Animation
{
    // Used to change animation characteristics during animation
    public class AnimationTransformation
    {
        public int FrameWaitMs { get; set; }

        /// <summary>
        /// The brightness to correct each pixel to. Must be between -1 (black) and 1 (white).
        /// </summary>
        public int BrightnessCorrection { get; set; }


        public AnimationTransformation(int initialFrameWaitMs)
        {
            FrameWaitMs = initialFrameWaitMs;
            BrightnessCorrection = 0;
        }

        public Color AdjustColor(Color color)
        {
            return AdjustBrightness(color);
        }

        private Color AdjustBrightness(Color color)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            int correctionFactor = BrightnessCorrection;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

    }
}
