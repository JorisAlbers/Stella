using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Fody.Helpers;
using StellaServer.Midi;
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

        public ReactiveCommand<Unit,long> BeatRegisterViaView { get; }
        public ReactiveCommand<Unit,Unit> Reset { get; }

        [Reactive]
        public IObservable<Unit> NextBeatObservable { get; private set; }
    
        
        public BpmViewModel(StellaServerLib.StellaServer stellaServer, MidiInputManager midiInputManager)
        {
            _stellaServer = stellaServer;

            var midiBeatRegisteredObservable = midiInputManager == null ? (Observable.Empty<long>()) : midiInputManager.RegisterBeat;
        
            bpmRecorder = new BpmRecorder();

            this.WhenAnyValue(x => x.bpmRecorder.Bpm).Subscribe(x => Bpm = x);
            this.WhenAnyValue(x => x.bpmRecorder.Interval).Subscribe(x => Interval = x);

            Reset = ReactiveCommand.Create<Unit, Unit>((x) => x);

            BeatRegisterViaView = ReactiveCommand.Create<Unit, long>((_) => Environment.TickCount);


            var beatRegistered = BeatRegisterViaView.Merge(midiBeatRegisteredObservable);

            // Act when either the view button or the midi controller button was pressed
            beatRegistered.Subscribe(x =>
            {
                bpmRecorder.OnNextBeat(x);
            });

            float originalBrightnessCorrection = float.MinValue;
            float[] originalRgbCorrection = null;

            // TODO use average interval + 1 second as throttle time
            beatRegistered.Where(x=>bpmRecorder.Interval != 0).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                var recorder = bpmRecorder;
                if (recorder.Interval == 0)
                {
                    return;
                }

                originalBrightnessCorrection = _stellaServer.Animator.StoryboardTransformationController.Settings
                    .MasterSettings.BrightnessCorrection;
                originalRgbCorrection = _stellaServer.Animator.StoryboardTransformationController.Settings
                    .MasterSettings.RgbFadeCorrection;

                bpmTimer?.Dispose();
                bpmTimer = new BpmTimer();
                bpmTimer.Start(recorder.Interval, recorder.Measurements);
                NextBeatObservable = bpmTimer.BeatObservable; // TODO switch when resetting
            });

          


            // The transformer
            bool toggle = false;
            this.WhenAnyObservable(x => x.NextBeatObservable).Subscribe(x =>
            {
                float ob = originalBrightnessCorrection;
                float[] oRgb = originalRgbCorrection;

                if (ob == float.MinValue || oRgb == null)
                {
                    return;
                }
                
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
                        _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(ob);
                        return;
                    }

                    var currentCorrection = _stellaServer.Animator.StoryboardTransformationController.Settings
                        .MasterSettings.RgbFadeCorrection;

                    switch (BpmTransformationMode)
                    {
                        case BpmTransformationMode.Reduce_Red:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { oRgb[0], currentCorrection[1], currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Green:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], oRgb[1], currentCorrection[2] });
                            break;
                        case BpmTransformationMode.Reduce_Blue:
                            _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(
                                new[] { currentCorrection[0], currentCorrection[1], oRgb[2] });
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                }
            });

            Reset.Subscribe(x =>
            {
                originalBrightnessCorrection = float.MinValue;
                originalRgbCorrection = null;
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