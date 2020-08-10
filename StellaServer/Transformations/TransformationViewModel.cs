using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Transformations
{
    public class TransformationViewModel
    {
        [Reactive] public float RedCorrection { get; set; }
        [Reactive] public float GreenCorrection { get; set; }
        [Reactive] public float BlueCorrection { get; set; }
        [Reactive] public float BrightnessCorrection { get; set; }

        [Reactive] public int TimeUnitsPerFrame { get; set; }

        /// <summary>
        /// TODO this one only adjust the master now, also add other animations
        /// </summary>
        /// <param name="stellaServer"></param>
        public TransformationViewModel(StellaServerLib.StellaServer stellaServer)
        {
            /*this.WhenAnyValue(x => RedCorrection,
                x => x.GreenCorrection,
                x => x.BlueCorrection,
                (r, g, b) => new float[] {r, g, b})
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext => stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(onNext));*/

            this.WhenAnyValue(x => x.TimeUnitsPerFrame)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext =>
                stellaServer.Animator.StoryboardTransformationController.SetTimeUnitsPerFrame(onNext));

            this.WhenAnyValue(x => x.BrightnessCorrection)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext => stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(onNext));
        }
    }
}
