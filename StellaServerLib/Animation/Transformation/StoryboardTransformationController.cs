using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServerLib.Animation.Transformation
{
    // Contains settings for a single storyboard.
    public class StoryboardTransformationController:  ReactiveObject
    {
        private AnimationTransformationSettings   _masterSettings;
        private AnimationTransformationSettings[] _animationSettings;

        [Reactive] public StoryboardTransformationSettings Settings { get; private set; }

        public StoryboardTransformationController(AnimationTransformationSettings masterSettings, AnimationTransformationSettings[] animationSettings)
        {
            _masterSettings = masterSettings;
            _animationSettings = animationSettings;

            Settings = new StoryboardTransformationSettings(masterSettings, animationSettings);
        }

        /// <summary>
        /// To be called in combination with init.
        /// </summary>
        /// <param name="animationSettings"></param>
        public StoryboardTransformationController(AnimationTransformationSettings[] animationSettings)
        {
            _animationSettings = animationSettings;
        }

        /// <summary>
        /// Sets the master transformation setting. Can only be called when the master is not yet set.
        /// </summary>
        /// <param name="masterSettings"></param>
        public void Init(AnimationTransformationSettings masterSettings)
        {
            if (_masterSettings != null)
            {
                throw new InvalidOperationException("Init can only be called when the master settings are not yet initialized."); ;
            }

            _masterSettings = masterSettings;
            Settings = new StoryboardTransformationSettings(masterSettings, _animationSettings);
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

            _masterSettings = new AnimationTransformationSettings(timeUnitsPerFrame, _masterSettings.BrightnessCorrection, _masterSettings.RgbFadeCorrection, _masterSettings.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings,_animationSettings);
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

            _animationSettings = (AnimationTransformationSettings[])_animationSettings.Clone();
            AnimationTransformationSettings old = _animationSettings[animationIndex];
            _animationSettings[animationIndex] = new AnimationTransformationSettings(timeUnitsPerFrame, old.BrightnessCorrection, old.RgbFadeCorrection, old.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
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
            
            _masterSettings = new AnimationTransformationSettings(_masterSettings.TimeUnitsPerFrame, brightnessCorrection, _masterSettings.RgbFadeCorrection, _masterSettings.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
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

            _animationSettings = (AnimationTransformationSettings[])_animationSettings.Clone();
            AnimationTransformationSettings old = _animationSettings[animationIndex];
            _animationSettings[animationIndex] = new AnimationTransformationSettings(old.TimeUnitsPerFrame, brightnessCorrection, old.RgbFadeCorrection, old.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
        }

        /// <summary>
        /// Set the rgb fade correction of the master
        /// </summary>
        /// <param name="rgbFadeCorrection"></param>
        public void SetRgbFadeCorrection(float[] rgbFadeCorrection)
        {
            if (rgbFadeCorrection.Any(x => x > 1 || x < 0))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            var previous = _masterSettings.RgbFadeCorrection;
            if (Math.Abs(rgbFadeCorrection[0] - previous[0]) < float.Epsilon &&
                Math.Abs(rgbFadeCorrection[1] - previous[1]) < float.Epsilon &&
                Math.Abs(rgbFadeCorrection[2] - previous[2]) < float.Epsilon)
            {
                // same.
                return;
            }

            _masterSettings = new AnimationTransformationSettings(_masterSettings.TimeUnitsPerFrame, _masterSettings.BrightnessCorrection, rgbFadeCorrection, _masterSettings.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
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

            // Create a new Array
            _animationSettings = (AnimationTransformationSettings[])_animationSettings.Clone();
            AnimationTransformationSettings old = _animationSettings[animationIndex];
            _animationSettings[animationIndex] = new AnimationTransformationSettings(old.TimeUnitsPerFrame, old.BrightnessCorrection, rgbFadeCorrection, old.IsPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
        }

        public void SetIsPaused(bool isPaused)
        {
            if (_masterSettings.IsPaused == isPaused)
            {
                return;
            }

            _masterSettings = new AnimationTransformationSettings(_masterSettings.TimeUnitsPerFrame, _masterSettings.BrightnessCorrection, _masterSettings.RgbFadeCorrection, isPaused);
            Settings = new StoryboardTransformationSettings(_masterSettings, _animationSettings);
        }

    }
}
