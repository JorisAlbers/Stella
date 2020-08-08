using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using StellaServer.Animation.Settings;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Details
{
    public class StoryboardDetailsControlViewModel : ReactiveObject
    {
        private readonly Storyboard _storyboard;
        private readonly BitmapRepository _bitmapRepository;

        public string Name { get; set; }

        public List<AnimationSettingViewModel> AnimationSettings { get; set; }
        
        public StoryboardDetailsControlViewModel(Storyboard storyboard, BitmapRepository bitmapRepository)
        {
            _storyboard = storyboard;
            _bitmapRepository = bitmapRepository;
            Name = storyboard.Name;
            AnimationSettings = storyboard.AnimationSettings.Select(x=> CreateAnimationSettingViewModel(x,_bitmapRepository)).ToList();
        }

        private AnimationSettingViewModel CreateAnimationSettingViewModel(IAnimationSettings settings, BitmapRepository bitmapRepository)
        {
            if (settings is BitmapAnimationSettings bitmapAnimation)
            {
                return new BitmapAnimationSettingsViewModel(bitmapAnimation, bitmapRepository);
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
