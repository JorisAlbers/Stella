using System;
using System.Linq;

namespace StellaServerLib.Animation.Transformation
{
    /// <summary>
    /// Keeps track of the current AnimationTransformation. Allows for thread safe access to the animationTransformation
    /// </summary>
    public class TransformationController
    {
        private AnimationTransformationSettings _masterAnimationTransformationSettings;
        private AnimationTransformationSettings[] _animationTransformationSettings;

        public AnimationTransformation AnimationTransformation { get; private set; }



        public TransformationController(AnimationTransformationSettings masterAnimationTransformationSettings, AnimationTransformationSettings[] animationTransformationSettings)
        {
            // Keep track of the setting
            _masterAnimationTransformationSettings = masterAnimationTransformationSettings;
            _animationTransformationSettings = animationTransformationSettings;

            // Create a new public AnimationTransformation
            AnimationTransformation = new AnimationTransformation(masterAnimationTransformationSettings, animationTransformationSettings);
        }
        
        /// <summary>
        /// Set the master frame wait ms
        /// </summary>
        public void SetTimeUnitsPerFrame(int timeUnitsPerFrame)
        {
            if (timeUnitsPerFrame < 0)
            {
                throw new ArgumentException($"The master timeUnitsPerFrame must be at least 0 ms.");
            }

            _masterAnimationTransformationSettings = new AnimationTransformationSettings(timeUnitsPerFrame, _masterAnimationTransformationSettings.BrightnessCorrection, _masterAnimationTransformationSettings.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
        }

        /// <summary>
        /// Set the frame wait ms of the animation at the specified index
        /// </summary>
        public void SetTimeUnitsPerFrame(int timeUnitsPerFrame, int animationIndex)
        {
            if (timeUnitsPerFrame < 0)
            {
                throw new ArgumentException($"The master timeUnitsPerFrame must be at least 0 ms.");
            }

            AnimationTransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new AnimationTransformationSettings(timeUnitsPerFrame, old.BrightnessCorrection, old.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
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

            _masterAnimationTransformationSettings = new AnimationTransformationSettings(_masterAnimationTransformationSettings.TimeUnitsPerFrame, brightnessCorrection, _masterAnimationTransformationSettings.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
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

            AnimationTransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new AnimationTransformationSettings(old.TimeUnitsPerFrame, brightnessCorrection, old.RgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
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

            _masterAnimationTransformationSettings = new AnimationTransformationSettings(_masterAnimationTransformationSettings.TimeUnitsPerFrame, _masterAnimationTransformationSettings.BrightnessCorrection, rgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
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

            AnimationTransformationSettings old = _animationTransformationSettings[animationIndex];
            _animationTransformationSettings[animationIndex] = new AnimationTransformationSettings(old.TimeUnitsPerFrame, old.BrightnessCorrection, rgbFadeCorrection);
            AnimationTransformation = new AnimationTransformation(_masterAnimationTransformationSettings, _animationTransformationSettings.ToArray());
        }
      

    }
}
