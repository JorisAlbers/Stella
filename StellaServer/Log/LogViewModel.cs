using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Log
{
    public class LogViewModel: ReactiveObject
    {
        [Reactive] public ObservableCollection<string> Messages { get; set; }

        public LogViewModel()
        {
            Messages = new ObservableCollection<string>();
            ConsoleOutWriter writer = new ConsoleOutWriter();
            Observable.FromEventPattern<EventHandler<string>, string>(
                    handler => writer.NewMessage+= handler,
                    handler => writer.NewMessage-= handler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext=>
                {
                    Messages.Add(onNext.EventArgs);
                    if (Messages.Count > 100)
                    {
                        Messages = new ObservableCollection<string>(Messages.Skip(50));
                    }
                });

            Console.SetOut(writer);

        }
    }
}