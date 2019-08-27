using System;

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


        public (float red, float green, float blue) AdjustColor(float red, float green, float blue)
        {
            // Red
            red = TransformChannel(red, RgbFadeCorrection[0]);
            // Green
            green = TransformChannel(green, RgbFadeCorrection[1]);
            // Blue
            blue = TransformChannel(blue, RgbFadeCorrection[2]);

            return (red, green, blue);
        }

        private float TransformChannel(float channel, float correctionFactor)
        {
            if (correctionFactor == 0)
            {
                return channel;
            }

            correctionFactor = 1 + correctionFactor;
            channel *= correctionFactor;

            return channel;
        }

        public (float red, float green, float blue) AdjustBrightness(float red, float green, float blue)
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

            return (red, green, blue);
        }
    }
}
