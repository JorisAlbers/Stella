using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Bpm;

namespace StellaServer
{
    public class BpmViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;

        [Reactive] private BpmRecorder bpmRecorder { get; set; }
        [Reactive] private BpmTimer bpmTimer { get; set; }

        [Reactive] public double Bpm { get; set; }
        [Reactive] public long Interval { get; set; }

        [Reactive] public BpmTransformationMode BpmTransformationMode { get; set; }

        public ReactiveCommand<Unit,long> RegisterBeat { get; }
        public ReactiveCommand<Unit,Unit> Reset { get; }

        [Reactive]
        public IObservable<Unit> NextBeatObservable { get; private set; }
    
        
        public BpmViewModel(StellaServerLib.StellaServer stellaServer)
        {
            _stellaServer = stellaServer;
            bpmRecorder = new BpmRecorder();

            this.WhenAnyValue(x => x.bpmRecorder.Bpm).Subscribe(x => Bpm = x);
            this.WhenAnyValue(x => x.bpmRecorder.Interval).Subscribe(x => Interval = x);

            Reset = ReactiveCommand.Create<Unit, Unit>((x) => x);

            RegisterBeat = ReactiveCommand.Create<Unit, long>((_) => Environment.TickCount);
            RegisterBeat.Subscribe(x =>
            {
                bpmRecorder.OnNextBeat(x);
            });

            // TODO use average interval + 1 second as throttle time
            RegisterBeat.Where(x=>bpmRecorder.Interval != 0).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                var recorder = bpmRecorder;
                if (recorder.Interval == 0)
                {
                    return;
                }

                bpmTimer?.Dispose();
                bpmTimer = new BpmTimer();
                bpmTimer.Start(recorder.Interval, recorder.Measurements);
                NextBeatObservable = bpmTimer.BeatObservable; // TODO switch when resetting
            });


            // The transformer
            bool toggle = false;
            this.WhenAnyObservable(x => x.NextBeatObservable).Subscribe(x =>
            {
                if (toggle)
                {
                    toggle = false;

                    if (BpmTransformationMode == BpmTransformationMode.Reduce_Brightness)
                    {
                        _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(-0.8f);
                        return;
                    }

                    var currentCorrection = _stellaServer.Animator.StoryboardTransformationController.Settings
                        .MasterSettings.RgbFadeCorrection;

                    switch (BpmTransformationMode)
                    {
                        case BpmTransformationMode.Reduce_Red:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { 0, currentCorrection[1], currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Green:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], 0, currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Blue:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], currentCorrection[1], 0 });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                }
                else
                {
                    toggle = true;
                    if (BpmTransformationMode == BpmTransformationMode.Reduce_Brightness)
                    {
                        _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(0);
                        return;
                    }

                    var currentCorrection = _stellaServer.Animator.StoryboardTransformationController.Settings
                        .MasterSettings.RgbFadeCorrection;

                    switch (BpmTransformationMode)
                    {
                        case BpmTransformationMode.Reduce_Red:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { 1, currentCorrection[1], currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Green:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], 1, currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Blue:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], currentCorrection[1], 1 });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                }
            });

            Reset.Subscribe(x =>
            {
                toggle = false;
                bpmTimer?.Dispose();
                bpmRecorder = new BpmRecorder();
            });

        }
    }

    public enum BpmTransformationMode
    {
        Reduce_Brightness,
        Reduce_Red,
        Reduce_Green,
        Reduce_Blue,

    }
}