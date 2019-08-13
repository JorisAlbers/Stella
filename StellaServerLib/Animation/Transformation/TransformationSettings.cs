using System;
using System.Drawing;

namespace StellaServerLib.Animation.Transformation
{
    public class TransformationSettings
    {
        public int FrameWaitMs { get; }

        /// <summary>
        /// The brightness to correct each pixel to. Must be between -1 (black) and 1 (white).
        /// </summary>
        public float BrightnessCorrection { get; }

        /// <summary>
        /// The fade correction for each rgb channel. Must be between -1 (color removed) and 0 (color present);
        /// </summary>
        public float[] RgbFadeCorrection { get; }

        
        public TransformationSettings(int frameWaitMs, float brightnessCorrection, float[] rgbFadeCorrection)
        {
            if (rgbFadeCorrection.Length != 3)
            {
                throw new ArgumentException($"Length of {nameof(rgbFadeCorrection)} must be 3");
            }

            FrameWaitMs = frameWaitMs;
            BrightnessCorrection = brightnessCorrection;
            RgbFadeCorrection = rgbFadeCorrection;
        }


        public Color AdjustColor(Color color)
        {
            // Red
            float red = TransformChannel(color.R, RgbFadeCorrection[0]);
            // Green
            float green = TransformChannel(color.G, RgbFadeCorrection[1]);
            // Blue
            float blue = TransformChannel(color.B, RgbFadeCorrection[2]);

            return Color.FromArgb((int)red, (int)green, (int)blue);
        }

        private float TransformChannel(byte channel, float correctionFactor)
        {
            float channelAsFloat = (float)channel;

            if (correctionFactor == 0)
            {
                return channelAsFloat;
            }

            correctionFactor = 1 + correctionFactor;
            channelAsFloat *= correctionFactor;

            return channelAsFloat;
        }

        public Color AdjustBrightness(Color color)
        {
            float correctionFactor = BrightnessCorrection;
            float red = color.R;
            float green = color.G;
            float blue = color.B;

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
