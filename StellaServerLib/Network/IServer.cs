using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Network
{
    public interface IServer : IDisposable
    {
        void Start(int broadcastPort, int udpPort, int remoteUdpPort, SocketConnectionCreator socketConnectionCreator,
            List<ClientMapping> clientMappings);

        /// <summary> Sends a frame to a client </summary>
        void SendToClient(int clientId, FrameWithoutDelta frame);

        event EventHandler<ClientStatusChangedEventArgs> ClientChanged;

    }

    public class ClientStatusChangedEventArgs : EventArgs
    {
        public ClientStatus Status { get; private set; }
        public int Id { get; }

        public ClientStatusChangedEventArgs(int id, ClientStatus status)
        {
            Id = id;
            Status = status;
        }
    }
}