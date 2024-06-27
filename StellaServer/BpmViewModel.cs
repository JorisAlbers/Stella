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


            Reset.Subscribe(x =>
            {
                bpmTimer?.Dispose();
                bpmRecorder = new BpmRecorder();
            });

        }
    }
}