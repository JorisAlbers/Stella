using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
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

        public bool UserCanSetStartTimes { get; }

        public ReactiveCommand<Unit, LayoutType> StartAtTheSameTime { get;  } = ReactiveCommand.Create<Unit, LayoutType>((_) => LayoutType.Straight);
        public ReactiveCommand<Unit, LayoutType> StartAsForwardSlash { get; } = ReactiveCommand.Create<Unit, LayoutType>((_) => LayoutType.Dash);
        public ReactiveCommand<Unit, LayoutType> StartAsArrowHead { get; } = ReactiveCommand.Create<Unit, LayoutType>((_) => LayoutType.ArrowHead);

        public IObservable<LayoutType> StartAnimation { get; }


        public List<AnimationSettingViewModel> AnimationSettings { get; set; }
        public ReactiveCommand<Unit, Unit> Back { get; set; } = ReactiveCommand.Create(() => { });

        public StoryboardDetailsControlViewModel(Storyboard storyboard, BitmapRepository bitmapRepository)
        {
            _storyboard = storyboard;
            _bitmapRepository = bitmapRepository;
            StartAnimation = Observable.Merge(StartAsArrowHead, StartAsForwardSlash, StartAtTheSameTime);
            UserCanSetStartTimes = storyboard.StartTimeCanBeAdjusted;
            Name = storyboard.Name;
            AnimationSettings = new List<AnimationSettingViewModel>();
            foreach (IAnimationSettings animationSetting in storyboard.AnimationSettings)
            {
                AnimationSettings.Add(CreateAnimationSettingViewModel(animationSetting,_bitmapRepository));
            }
        }

        private AnimationSettingViewModel CreateAnimationSettingViewModel(IAnimationSettings settings, BitmapRepository bitmapRepository)
        {
            if (settings is BitmapAnimationSettings bitmapAnimation)
            {
                foreach (BitmapAnimationSettingsViewModel viewmodel in AnimationSettings.OfType<BitmapAnimationSettingsViewModel>().Where(x=>x.BitmapName == bitmapAnimation.ImageName))
                {
                    // Use this image for mem efficiency
                    return new BitmapAnimationSettingsViewModel(bitmapAnimation, viewmodel.Bitmap);
                }
                
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
