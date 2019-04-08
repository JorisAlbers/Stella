using System;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaClient.Network
{
    public interface IStellaServer
    {
        void Dispose();
        void Send(MessageType type, byte[] message);
        void SendFrameRequest(int lastFrameIndex, int count);
        void Start();
        event EventHandler<FrameSetMetadata> AnimationStartReceived;
        event EventHandler<Frame> FrameReceived;

    }
}