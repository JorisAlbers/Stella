using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Midi;
using StellaServerLib.Animation;

namespace StellaServer.Transformations
{
    public class TransformationViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public int RedCorrection { get; set; } = 100;
        [Reactive] public int GreenCorrection { get; set; } = 100;
        [Reactive] public int BlueCorrection { get; set; } = 100;
        [Reactive] public int BrightnessCorrection { get; set; }

        [Reactive] public int TimeUnitsPerFrame { get; set; } = 10;

        public ReactiveCommand<Unit,Unit> Reset { get; }
        public ReactiveCommand<Unit,Unit> Stop { get; }

        [Reactive] public bool ShouldPause { get; set; }
        public BpmViewModel BpmViewModel { get; set; }

        /// <summary>
        /// TODO this one only adjust the master now, also add other animations
        /// </summary>
        /// <param name="stellaServer"></param>
        public TransformationViewModel(StellaServerLib.StellaServer stellaServer, IAnimation clearAnimation, MidiInputManager midiInputManager)
        {
            _stellaServer = stellaServer;
            BpmViewModel = new BpmViewModel(stellaServer, midiInputManager);

            this.WhenAnyValue(
                x => x.RedCorrection,
                x => x.GreenCorrection,
                x => x.BlueCorrection,
                (r, g, b) => new int[] {r, g, b})
                .Throttle(TimeSpan.FromMilliseconds(100))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext => stellaServer.Animator?.StoryboardTransformationController.SetRgbFadeCorrection(IntegerCorrectionToFloatCorrection(onNext)));

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

            this.WhenAnyValue(x =>
                    x._stellaServer.Animator.StoryboardTransformationController.Settings.MasterSettings
                        .RgbFadeCorrection)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    RedCorrection = ConvertToSlider(x[0]);
                    GreenCorrection = ConvertToSlider(x[1]);
                    BlueCorrection = ConvertToSlider(x[2]);
                });

            this.WhenAnyValue(x =>
                    x._stellaServer.Animator.StoryboardTransformationController.Settings.MasterSettings
                        .BrightnessCorrection)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    BrightnessCorrection = ConvertToSlider(x);
                });

            this.WhenAnyValue(x =>
                    x._stellaServer.Animator.StoryboardTransformationController.Settings.MasterSettings
                        .TimeUnitsPerFrame)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    TimeUnitsPerFrame = x;
                });


            this.Reset = ReactiveCommand.Create(() =>
            {
                RedCorrection = 100;
                GreenCorrection = 100;
                BlueCorrection = 100;
                BrightnessCorrection = 0;
                TimeUnitsPerFrame = 10;
            });

            this.Stop = ReactiveCommand.Create(() =>
            {

            });

            this.WhenAnyValue(x => x.ShouldPause).Subscribe(x =>
            {
                _stellaServer.Animator?.StoryboardTransformationController.SetIsPaused(x);
            });

            this.WhenAnyValue(x => x._stellaServer.Animator.StoryboardTransformationController.Settings.MasterSettings
                .IsPaused)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x =>
                {
                    ShouldPause = x;
                });
        }

        private float[] IntegerCorrectionToFloatCorrection(int[] i)
        {
            // i ranges between 100 and -100
            return i.Select(IntegerCorrectionToFloatCorrection).ToArray();
        }

        private float IntegerCorrectionToFloatCorrection(int i)
        {
            // In the end,
            // 0 = completely removed
            // 1 = completely on

            // In the slider, 100 = on, 0 = removed.

            // i ranges between 0 and -100
            return (float) (i / 100.0);
        }


        private int ConvertToSlider(float f)
        {
            float x = f * 100.0f;

            return (int)x;
        }

    }
}
