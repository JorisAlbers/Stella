using System.Drawing;
using System.Windows.Media;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;
using Color = System.Drawing.Color;

namespace StellaServer.Animation.Settings
{
    public class FadingPulseAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public Color Color { get; set; }
        [Reactive] public int FadeSteps { get; set; }
        

        public FadingPulseAnimationSettingsViewModel(FadingPulseAnimationSettings animationSettings) : base(animationSettings)
        {
            Color = animationSettings.Color;
            FadeSteps = animationSettings.FadeSteps;
        }
    }
}