using System.Drawing;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class BitmapAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public string BitmapName { get; set; }
        [Reactive] public Bitmap Bitmap { get; set; }
        [Reactive] public bool Wraps { get; set; }

        public BitmapAnimationSettingsViewModel(BitmapAnimationSettings animationSettings, BitmapRepository bitmapRepository) : base(animationSettings)
        {
            BitmapName = animationSettings.ImageName;
            Wraps = animationSettings.Wraps;
            Bitmap = bitmapRepository.Load(animationSettings.ImageName);
        }
    }
}