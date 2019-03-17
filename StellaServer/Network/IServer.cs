using System;
using StellaLib.Network;

namespace StellaServer.Network
{
    public interface IServer
    {
        void Start();
        string[] ConnectedClients{get;}
        int NewConnectionsCount{get;}
        void SendMessageToClient(string clientID, MessageType messageType, byte[] message);
        event EventHandler<AnimationRequestEventArgs> AnimationRequestReceived;
    }
}