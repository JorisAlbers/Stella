using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Transformations
{
    public class TransformationViewModel : ReactiveObject
    {
        [Reactive] public int RedCorrection { get; set; }
        [Reactive] public int GreenCorrection { get; set; }
        [Reactive] public int BlueCorrection { get; set; }
        [Reactive] public int BrightnessCorrection { get; set; }

        [Reactive] public int TimeUnitsPerFrame { get; set; }

        /// <summary>
        /// TODO this one only adjust the master now, also add other animations
        /// </summary>
        /// <param name="stellaServer"></param>
        public TransformationViewModel(StellaServerLib.StellaServer stellaServer)
        {
            this.WhenAnyValue(
                x => x.RedCorrection,
                x => x.GreenCorrection,
                x => x.BlueCorrection,
                (r, g, b) => new int[] {r, g, b})
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext => stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(IntegerCorrectionToFloatCorrection(onNext)));

            this.WhenAnyValue(x => x.TimeUnitsPerFrame)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext =>
                    stellaServer.Animator?.StoryboardTransformationController.SetTimeUnitsPerFrame(onNext));

            this.WhenAnyValue(x => x.BrightnessCorrection)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext =>
                    stellaServer.Animator?.StoryboardTransformationController.SetBrightnessCorrection(IntegerCorrectionToFloatCorrection(onNext)));
        }

        private float[] IntegerCorrectionToFloatCorrection(int[] i)
        {
            // i ranges between 10 and -10
            return i.Select(IntegerCorrectionToFloatCorrection).ToArray();
        }

        private float IntegerCorrectionToFloatCorrection(int i)
        {
            // i ranges between 10 and -10
            return (float) (i / 10.0);
        }


    }
}
