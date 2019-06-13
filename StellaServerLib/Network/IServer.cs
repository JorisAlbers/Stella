using System;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public interface IServer
    {
        void Start();
        int[] ConnectedClients{get;}
        int NewConnectionsCount{get;}
        void SendMessageToClient(int clientID, MessageType messageType, byte[] message);
    }
}