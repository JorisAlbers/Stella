using System.Drawing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class RepeatingPatternsAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public Color[][] Patterns { get; set; }

        public RepeatingPatternsAnimationSettingsViewModel(RepeatingPatternAnimationSettings animationSettings) : base(animationSettings)
        {
            Patterns = animationSettings.Patterns;
        }
    }
}