using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public abstract class AnimationSettingViewModel : ReactiveObject
    {
        [Reactive] public int StartIndex { get; set; }
        [Reactive] public int StripLength { get; set; }
        [Reactive] public int TimeUnitsPerFrame { get; set; }
        /// <summary> The time at which this animation will start. In milliseconds.  </summary>
        [Reactive] public int RelativeStart { get; set; }
        
        public AnimationSettingViewModel(IAnimationSettings animationSettings)
        {
            StartIndex = animationSettings.StartIndex;
            StripLength = animationSettings.StripLength;
            TimeUnitsPerFrame = animationSettings.TimeUnitsPerFrame;
            RelativeStart = animationSettings.RelativeStart;
        }
    }
}