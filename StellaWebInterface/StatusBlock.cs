using System;
using System.Reactive.Linq;
using DotNetify;

namespace StellaWebInterface
{
    public class StatusBlock : BaseVM
    {
        private int connectedClients = 0;
        public int ConnectedClients
        {
            get => connectedClients;
            set => connectedClients = value; 
        }

        private int connectedRaspberries = 0;
        public int ConnectedRaspberries
        {
            get => connectedRaspberries;
            set
            {
                Console.Out.WriteLine("Value is upped");
                connectedRaspberries = value;
                connectedClients += 5;
                Changed(nameof(ConnectedClients));
                Changed(nameof(connectedRaspberries));
                PushUpdates();
            }
        }

        public StatusBlock()
        {
        }
    }
}
