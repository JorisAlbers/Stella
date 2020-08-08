using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using StellaServer.Animation.Settings;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Details
{
    public class StoryboardDetailsControlViewModel : ReactiveObject
    {
        private readonly Storyboard _storyboard;

        public string Name { get; set; }

        public List<AnimationSettingViewModel> AnimationSettings { get; set; }
        
        public StoryboardDetailsControlViewModel(Storyboard storyboard)
        {
            _storyboard = storyboard;
            Name = storyboard.Name;
            AnimationSettings = storyboard.AnimationSettings.Select(CreateAnimationSettingViewModel).ToList();
        }

        private AnimationSettingViewModel CreateAnimationSettingViewModel(IAnimationSettings settings)
        {
            if (settings is BitmapAnimationSettings bitmapAnimation)
            {
                return new BitmapAnimationSettingsViewModel(bitmapAnimation);
            }
            if (settings is MovingPatternAnimationSettings movingPatternSetting)
            {
                return new MovingPatternAnimationSettingsViewModel(movingPatternSetting);
            }
            if (settings is SlidingPatternAnimationSettings slidingPatternSetting)
            {
                return new SlidingPatternAnimationSettingsViewModel(slidingPatternSetting);
            }
            if (settings is RepeatingPatternAnimationSettings repeatingPatternSetting)
            {
                return new RepeatingPatternsAnimationSettingsViewModel(repeatingPatternSetting);
            }
            if (settings is RandomFadeAnimationSettings randomFadeSetting)
            {
                return new RandomFadeAnimationSettingsViewModel(randomFadeSetting);
            }
            if (settings is FadingPulseAnimationSettings fadingPulseSetting)
            {
                return new FadingPulseAnimationSettingsViewModel(fadingPulseSetting);
            }
            
            throw new NotImplementedException();
        }

    }
}
