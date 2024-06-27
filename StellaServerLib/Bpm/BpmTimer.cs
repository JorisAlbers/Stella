using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using ReactiveUI;

namespace StellaServerLib.Bpm
{
    public class BpmTimer : ReactiveObject, IDisposable
    {
        private Subject<Unit> _beatSubject = new Subject<Unit>();
        private long _nextBeatAt;
        private IDisposable _disposable;


        public IObservable<Unit> BeatObservable => _beatSubject;

        public void Start(long interval, List<long> measurements)
        {
            // TODO correct for errors in the measurements.
            // TODO for now, use the second last measurements as this measurement is often more accurate as the last one.

            long lastBeatAt = measurements[^2];
            _nextBeatAt = lastBeatAt + interval;

            // start in 500 ms
            while (Environment.TickCount + 500 > _nextBeatAt)
            {
                _nextBeatAt += interval;
            }

            // TODO implement Wait time  to add some buffer
            _disposable = Observable.Timer(
                TimeSpan.FromMilliseconds(_nextBeatAt - Environment.TickCount),
                TimeSpan.FromMilliseconds(interval))
                .Subscribe(x=>
                {
                    _beatSubject.OnNext(Unit.Default);
                });
        }


        public void Dispose()
        {
            _disposable.Dispose();
        }
    }
}