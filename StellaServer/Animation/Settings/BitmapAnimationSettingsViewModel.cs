using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class BitmapAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public string BitmapName { get; set; }
        [Reactive] public bool Wraps { get; set; }

        public BitmapAnimationSettingsViewModel(BitmapAnimationSettings animationSettings) : base(animationSettings)
        {
            BitmapName = animationSettings.ImageName;
            Wraps = animationSettings.Wraps;
        }
    }
}