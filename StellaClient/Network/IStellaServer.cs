using System;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaClient.Network
{
    public interface IStellaServer
    {
        void Dispose();
        void Send(MessageType type, byte[] message);
        void Start();
        event EventHandler<FrameWithoutDelta> FrameReceived;
        event EventHandler RenderFrameReceived;


    }
}