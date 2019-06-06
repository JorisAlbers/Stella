using System;
using StellaLib.Network;

namespace StellaServer.Network
{
    public interface IServer
    {
        void Start();
        int[] ConnectedClients{get;}
        int NewConnectionsCount{get;}
        void SendMessageToClient(int clientID, MessageType messageType, byte[] message);
        event EventHandler<AnimationRequestEventArgs> AnimationRequestReceived;
    }
}