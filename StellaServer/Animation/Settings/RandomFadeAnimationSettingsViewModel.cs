using System.Drawing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class RandomFadeAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public Color[] Pattern { get; set; }
        [Reactive] public int FadeSteps { get; set; }

        public RandomFadeAnimationSettingsViewModel(RandomFadeAnimationSettings animationSettings) : base(animationSettings)
        {
            Pattern = animationSettings.Pattern;
            FadeSteps = animationSettings.FadeSteps;
        }
    }
}