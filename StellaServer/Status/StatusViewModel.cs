using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Status
{
    public class StatusViewModel : ReactiveObject
    {
        [Reactive] public ClientStatusViewModel[] Clients { get; set; }
        [Reactive] public string CurrentlyPlaying { get; set; }

        public StatusViewModel(StellaServerLib.StellaServer stellaServer, int numberOfClients)
        {
            Clients = new ClientStatusViewModel[numberOfClients];
            for (int i = 0; i < numberOfClients; i++)
            {
                Clients[i] = new ClientStatusViewModel($"client {i}");
            }
        }
    }
}
