using ReactiveUI;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Reactive;
using System.Reactive.Subjects;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Bpm;

namespace StellaServer
{
    public class BpmViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        private Subject<bool> _nextBeatSubject = new Subject<bool>();

        [Reactive] private BpmRecorder bpmRecorder { get; set; }

        [Reactive] public double Bpm { get; set; }
        [Reactive] public long Interval { get; set; }

        public ReactiveCommand<Unit,long> RegisterBeat { get; }
        public ReactiveCommand<Unit,Unit> Reset { get; }

        public IObservable<bool> NextBeatObservable => _nextBeatSubject;
        
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

            Reset.Subscribe(x =>
            {
                bpmRecorder = new BpmRecorder();
            });

        }
    }
}