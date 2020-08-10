using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Animation;

namespace StellaServer.Status
{
    public class StatusViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public List<ClientStatusViewModel> Clients { get; set; }
        [Reactive] public string CurrentlyPlaying { get; set; }

        public StatusViewModel(StellaServerLib.StellaServer stellaServer, int expectedNumberOfClients)
        {
            _stellaServer = stellaServer;
            Clients = new List<ClientStatusViewModel>();
            for (int i = 0; i < expectedNumberOfClients; i++)
            {
                Clients.Add(new ClientStatusViewModel($"client {i}"));
            }
        }

        public void AnimationStarted(IAnimation animation)
        {
            CurrentlyPlaying = animation.Name;
        }
    }
}
