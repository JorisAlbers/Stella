using System;
using System.Linq;

namespace StellaServerLib.Animation.Transformation
{
    /// <summary>
    /// Keeps track of the current AnimationTransformation. Allows for thread safe access to the animationTransformation
    /// </summary>
    public class TransformationController
    {
        private TransformationSettings _masterTransformationSettings;
        private TransformationSettings[] _animationTransformationSettings;

        public AnimationTransformation AnimationTransformation { get; private set; }



        public TransformationController(TransformationSettings masterTransformationSettings, TransformationSettings[] animationTransformationSettings)
        {
            // Keep track of the setting
            _masterTransformationSettings = masterTransformationSettings;
            _animationTransformationSettings = animationTransformationSettings;

            // Create a new public AnimationTransformation
            AnimationTransformation = new AnimationTransformation(masterTransformationSettings, animationTransformationSettings);
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

            _masterTransformationSettings = new TransformationSettings(frameWaitMs, _masterTransformationSettings.BrightnessCorrection, _masterTransformationSettings.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }

        /// <summary>
        /// Set the frame wait ms of the animation at the specified index
        /// </summary>
        public void SetFrameWaitMs(int frameWaitMs, int animationIndex)
        {
            if (frameWaitMs < 0)
            {
                throw new ArgumentException($"The master frameWaitMs must be at least 0 ms.");
            }

            TransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new TransformationSettings(frameWaitMs, old.BrightnessCorrection, old.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }

        /// <summary>
        /// Set the master brightness correction
        /// </summary>
        /// <param name="brightnessCorrection"></param>
        public void SetBrightnessCorrection(float brightnessCorrection)
        {
            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException($"The brightness correction must in the range of -1 and 1");
            }

            _masterTransformationSettings = new TransformationSettings(_masterTransformationSettings.FrameWaitMs, brightnessCorrection, _masterTransformationSettings.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }

        /// <summary>
        /// Set the brightness correction of the animation at the specified index
        /// </summary>
        public void SetBrightnessCorrection(float brightnessCorrection, int animationIndex)
        {
            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException($"The brightness correction must in the range of -1 and 1");
            }

            TransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new TransformationSettings(old.FrameWaitMs, brightnessCorrection, old.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }
        
        /// <summary>
        /// Set the rgb fade correction of the master
        /// </summary>
        /// <param name="rgbFadeCorrection"></param>
        public void SetRgbFadeCorrection(float[] rgbFadeCorrection)
        {
            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            _masterTransformationSettings = new TransformationSettings(_masterTransformationSettings.FrameWaitMs, _masterTransformationSettings.BrightnessCorrection, rgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }

        /// <summary>
        /// Set the rgb fade correction of the animation at the specified index
        /// </summary>
        public void SetRgbFadeCorrection(float[] rgbFadeCorrection, int animationIndex)
        {
            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            TransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new TransformationSettings(old.FrameWaitMs, old.BrightnessCorrection, rgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterTransformationSettings, _animationTransformationSettings.ToArray());
        }
      

    }
}
