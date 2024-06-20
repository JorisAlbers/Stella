using System;

namespace StellaServerLib.Animation.Transformation
{
    public class AnimationTransformationSettings
    {
        public int TimeUnitsPerFrame { get; }

        /// <summary>
        /// The brightness to correct each pixel to. Must be between -1 (black) and 1 (white).
        /// </summary>
        public float BrightnessCorrection { get; }

        /// <summary>
        /// The fade correction for each rgb channel. Must be between -1 (color removed) and 0 (color present);
        /// </summary>
        public float[] RgbFadeCorrection { get; }

        /// <summary>
        /// If the animation is paused
        /// </summary>
        public bool IsPaused { get; }

        
        public AnimationTransformationSettings(int timeUnitsPerFrame, float brightnessCorrection, float[] rgbFadeCorrection, bool isPaused)
        {
            if (rgbFadeCorrection.Length != 3)
            {
                throw new ArgumentException($"Length of {nameof(rgbFadeCorrection)} must be 3");
            }

            TimeUnitsPerFrame = timeUnitsPerFrame;
            BrightnessCorrection = brightnessCorrection;
            RgbFadeCorrection = rgbFadeCorrection;
            IsPaused = isPaused;
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
            if (Math.Abs(correctionFactor - 1) < Single.Epsilon)
            {
                return channel;
            }

            channel *= correctionFactor;

            return channel;
        }

        public (float red, float green, float blue) AdjustBrightness(float red, float green, float blue)
        {
            float correctionFactor = BrightnessCorrection;

            // do not adjust near black colors
            if (red < 1 && green < 1 && blue < 1)
            {
                return (red, green, blue);
            }

            if (correctionFactor < 0)
            {
                //make color darker, changer brightness
                correctionFactor += 1;
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
