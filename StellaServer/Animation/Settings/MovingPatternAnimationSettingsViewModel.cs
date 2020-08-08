using System.Drawing;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class MovingPatternAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public Color[] Pattern { get; set; }

        public MovingPatternAnimationSettingsViewModel(MovingPatternAnimationSettings animationSettings) : base(animationSettings)
        {
            Pattern = animationSettings.Pattern;
        }
    }
}