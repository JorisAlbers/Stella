using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        public float BrightnessCorrection { get; set; }

        /// <summary>
        /// The fade correction for each rgb channel. Must be between -1 (color removed) and 0 (color present);
        /// </summary>
        public float[] RgbFadeCorrection { get; set; } = new float[3];


        public AnimationTransformation(int initialFrameWaitMs)
        {
            FrameWaitMs = initialFrameWaitMs;
            BrightnessCorrection = 0;
        }

        public Color AdjustColor(Color color)
        {
            // Red
            float red = TransformChannel(color.R, RgbFadeCorrection[0]);
            // Green
            float green = TransformChannel(color.G, RgbFadeCorrection[1]);
            // Blue
            float blue = TransformChannel(color.B, RgbFadeCorrection[2]);

            return AdjustBrightness(red, green, blue);
        }

        private float TransformChannel(byte channel, float correctionFactor)
        {
            float channelAsFloat = (float) channel;

            if (correctionFactor == 0)
            {
                return channelAsFloat;
            }

            correctionFactor = 1 + correctionFactor;
            channelAsFloat *= correctionFactor;
           
            return channelAsFloat;
        }

        


        private Color AdjustBrightness(float red, float green, float blue)
        {
            float correctionFactor = BrightnessCorrection;

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
            
            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

    }
}
