using System;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaClientLib.Network
{
    public interface IStellaServer
    {
        void Dispose();
        void Send(MessageType type, byte[] message);
        void Start();
        event EventHandler<FrameWithoutDelta> RenderFrameReceived;


    }
}