using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation
{
    public class AnimationPanelItemViewModel : ReactiveObject
    {
        public IAnimation Animation { get; }

        public string Name { get; set; }

        public ReactiveCommand<Unit, Unit> StartCommand { get; } = ReactiveCommand.Create(() => { });
        public ReactiveCommand<LayoutType, IAnimation> StartWithLayout { get; }

        public bool IsPlayList { get; set; }

        public ReactiveCommand<int, int> SendToPad { get; }


        public AnimationPanelItemViewModel(IAnimation animation)
        {
            Animation = animation;
            Name = animation.Name;
            IsPlayList = animation is PlayList;
            SendToPad = ReactiveCommand.Create<int, int>(i => i);

            if (animation.StartTimeCanBeAdjusted)
            {
                StartWithLayout = ReactiveCommand.Create<LayoutType, IAnimation>((x) =>
                {
                    if (animation is Storyboard sb)
                    {
                        BitmapAnimationSettings[] settings =
                            sb.AnimationSettings.OfType<BitmapAnimationSettings>().ToArray();

                        if (settings.Length != sb.AnimationSettings.Length)
                        {
                            throw new ArgumentException(
                                "All animation settings must be of type bitmapanimationsettings");
                        }

                        switch (x)
                        {
                            case LayoutType.Unknown:
                            case LayoutType.Straight:
                                settings = BitmapStoryboardCreator.StartAtTheSameTime(settings);
                                break;
                            case LayoutType.ArrowHead:
                                settings = BitmapStoryboardCreator.StartAsArrowHead(settings, 500);
                                break;
                            case LayoutType.Dash:
                                settings = BitmapStoryboardCreator.StartAsDash(settings, 500);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(x), x, null);
                        }

                        return new Storyboard()
                        {
                            AnimationSettings = settings,
                            Name = Animation.Name,
                        };
                    }

                    throw new ArgumentException("Animation start time cannot be adjusted, use the start command");

                });
            }
        }
    }
}