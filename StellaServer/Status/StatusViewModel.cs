using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Animation;
using StellaServerLib.Network;

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

            _stellaServer.ClientStatusChanged += StellaServerOnClientStatusChanged; 
        }

        private void StellaServerOnClientStatusChanged(object sender, ClientStatusChangedEventArgs e)
        {
            // id is index
            Clients[e.Id].IsConnected = e.Status == ClientStatus.Connected;
        }

        public void AnimationStarted(IAnimation animation)
        {
            CurrentlyPlaying = animation.Name;
        }
    }
}
