using System;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public interface IServer : IDisposable
    {
        void Start();

        /// <summary> Sends a simple message to a client with only a message type </summary>
        void SendToClient(int clientId, MessageType messageType);

        /// <summary> Sends a frame to a client </summary>
        void SendToClient(int clientId, FrameWithoutDelta frame);

    }
}