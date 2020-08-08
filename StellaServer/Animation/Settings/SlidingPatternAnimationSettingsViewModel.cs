using System.Drawing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class SlidingPatternAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public Color[] Pattern { get; set; }

        public SlidingPatternAnimationSettingsViewModel(SlidingPatternAnimationSettings animationSettings) : base(animationSettings)
        {
            Pattern = animationSettings.Pattern;
        }
    }
}