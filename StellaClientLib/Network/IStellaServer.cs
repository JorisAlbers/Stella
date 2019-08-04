using System;
using System.Net;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaClientLib.Network
{
    public interface IStellaServer
    {
        void Dispose();
        void Send(MessageType type, byte[] message);
        void Start(IPEndPoint serverAdress, int udpPort, int ID);
        event EventHandler<FrameWithoutDelta> RenderFrameReceived;


    }
}