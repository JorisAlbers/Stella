using System;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public interface IServer : IDisposable
    {
        void Start(string ip, int port, int udpPort, int remoteUdpPort);

        /// <summary> Sends a simple message to a client with only a message type </summary>
        void SendToClient(int clientId, MessageType messageType);

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