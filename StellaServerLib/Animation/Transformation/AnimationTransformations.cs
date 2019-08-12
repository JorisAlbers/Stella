using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StellaServerLib.Animation.Transformation
{
    // Used to change animation characteristics during animation
    public class AnimationTransformation
    {
        private readonly TransformationSettings _masterTransformationSettings;

        private readonly List<TransformationSettings> _animationsTransformationSettings;


        public AnimationTransformation(TransformationSettings masterTransformationSettings)
        {
            _masterTransformationSettings = masterTransformationSettings;
            _animationsTransformationSettings = new List<TransformationSettings>();
        }

        public void AddTransformationSettings(int initialFrameWaitMs)
        {
            _animationsTransformationSettings.Add(new TransformationSettings(initialFrameWaitMs));
        }

        /// <summary>
        /// The master frame wait MSs
        /// </summary>
        /// <returns></returns>
        public int GetFrameWaitMs()
        {
            return _masterTransformationSettings.FrameWaitMs;
        }

        /// <summary>
        /// The master frame wait ms + the frame wait ms of the animation at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public int GetFrameWaitMs(int index)
        {
            if (index < 0 || index >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {index}");
            }

            return _masterTransformationSettings.FrameWaitMs + _animationsTransformationSettings[index].FrameWaitMs;
        }

        /// <summary>
        /// Set the master frame wait ms
        /// </summary>
        public void SetFrameWaitMs(int frameWaitMs)
        {
            if (frameWaitMs < 0)
            {
                throw new ArgumentException($"The master frameWaitMs must be at least 0 ms.");
            }

            _masterTransformationSettings.FrameWaitMs = frameWaitMs;
        }

        /// <summary>
        /// Set the frame wait ms of the animation at the specified index
        /// </summary>
        public void SetFrameWaitMs(int frameWaitMs, int animationIndex)
        {
            if (frameWaitMs < 1)
            {
                throw new ArgumentException($"The frameWaitMs must be at least 1 ms.");
            }

            if (animationIndex < 0 || animationIndex >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            _animationsTransformationSettings[animationIndex].FrameWaitMs = frameWaitMs;
        }

        /// <summary>
        /// Get the master BrightnessCorrection
        /// </summary>
        /// <returns></returns>
        public float GetBrightnessCorrection()
        {
            return _masterTransformationSettings.BrightnessCorrection;
        }

        /// <summary>
        /// Get the BrightnessCorrection of the animation at the specified index
        /// </summary>
        /// <returns></returns>
        public float GetBrightnessCorrection(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            return _animationsTransformationSettings[animationIndex].BrightnessCorrection;
        }

        /// <summary>
        /// Set the master brightness correction
        /// </summary>
        /// <param name="brightnessCorrection"></param>
        public void SetBrightnessCorrection(float brightnessCorrection)
        {
            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException("The brightness correction must be between -1 and 1");
            }

            _masterTransformationSettings.BrightnessCorrection = brightnessCorrection;
        }


        /// <summary>
        /// Set the brightness correction of the animation at the specified index
        /// </summary>
        public void SetBrightnessCorrection(float brightnessCorrection, int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException("The brightness correction must be between -1 and 1");
            }

            _animationsTransformationSettings[animationIndex].BrightnessCorrection = brightnessCorrection;
        }

        /// <summary>
        /// Get the master RGB fade correction
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public float[] GetRgbFadeCorrection()
        {
            return _masterTransformationSettings.RgbFadeCorrection;
        }

        /// <summary>
        /// Get the RGB fade correction of the animation at the specified index
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public float[] GetRgbFadeCorrection(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            return _animationsTransformationSettings[animationIndex].RgbFadeCorrection;
        }

        public void SetRgbFadeCorrection(float[] rgbFadeCorrection)
        {
            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            _masterTransformationSettings.RgbFadeCorrection = rgbFadeCorrection;
        }

        public void SetRgbFadeCorrection(float[] rgbFadeCorrection, int animationIndex)
        {
            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            if (animationIndex < 0 || animationIndex >= _animationsTransformationSettings.Count)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            _animationsTransformationSettings[animationIndex].RgbFadeCorrection = rgbFadeCorrection;
        }

        public Color AdjustColor(int index, Color color)
        {
            // TODO slow and not thread safe
            // Adjust RGB
            color = _masterTransformationSettings.AdjustColor(color);
            color = _animationsTransformationSettings[index].AdjustColor(color);

            // Adjust Brightness
            color = _masterTransformationSettings.AdjustBrightness(color);
            return _animationsTransformationSettings[index].AdjustBrightness(color);
        }

        /// <summary>
        /// Adds the master frame wait ms to the animation frame wait ms
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public int GetCorrectedFrameWaitMs(int animationIndex)
        {
            return _masterTransformationSettings.FrameWaitMs +
                   _animationsTransformationSettings[animationIndex].FrameWaitMs;
        }
    }
}
