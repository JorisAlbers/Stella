using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Log;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServer.Status
{
    public class StatusViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public List<ClientStatusViewModel> Clients { get; set; }
        [Reactive] public string CurrentlyPlaying { get; set; }
        
        public ReactiveCommand<Unit, Unit> OpenLog { get; }

        public StatusViewModel(StellaServerLib.StellaServer stellaServer, int expectedNumberOfClients,
            LogViewModel logViewModel)
        {
            _stellaServer = stellaServer;
            Clients = new List<ClientStatusViewModel>();
            for (int i = 0; i < expectedNumberOfClients; i++)
            {
                Clients.Add(new ClientStatusViewModel($"client {i}"));
            }
            
            Observable.FromEventPattern<EventHandler<ClientStatusChangedEventArgs>, ClientStatusChangedEventArgs>(
                    handler => _stellaServer.ClientStatusChanged += handler,
                    handler => _stellaServer.ClientStatusChanged -= handler)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext =>
                {
                    // id is index
                    Clients[onNext.EventArgs.Id].IsConnected = onNext.EventArgs.Status == ClientStatus.Connected;
                });

             OpenLog   = ReactiveCommand.Create(() =>
            {
                var window = new LogWindow();
                window.ViewModel = logViewModel;
                window.Show();
            });
        }

        public void AnimationStarted(IAnimation animation)
        {
            CurrentlyPlaying = animation.Name;
        }
    }
}
