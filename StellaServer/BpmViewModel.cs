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
        private double _bpm;
        private long _interval;
        private BpmRecorder _bpmRecorder;
        private bool _animationToggle;
        private Subject<bool> _nextBeatSubject = new Subject<bool>();
        

        [Reactive] public double Bpm { get; set; }
        [Reactive] public long Interval { get; set; }

        public ReactiveCommand<Unit,long> RegisterBeat { get; }

        public IObservable<bool> NextBeatObservable => _nextBeatSubject;
        
        public BpmViewModel(StellaServerLib.StellaServer stellaServer)
        {
            _stellaServer = stellaServer;
            _bpmRecorder = new BpmRecorder();
            _bpmRecorder.PropertyChanged += BpmRecorderOnPropertyChanged;


            RegisterBeat = ReactiveCommand.Create<Unit, long>((_) => Environment.TickCount);
            RegisterBeat.Subscribe(x =>
            {
                _bpmRecorder.OnNextBeat(x);
            });
        }

        private void BpmRecorderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BpmRecorder.Bpm):
                    Bpm = _bpmRecorder.Bpm;
                    break;
                case nameof(BpmRecorder.Interval):
                    Interval = _bpmRecorder.Interval;
                    break;
            }
        }
    }
}