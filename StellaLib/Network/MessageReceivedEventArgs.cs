using System;

namespace StellaLib.Network
{
    /// <typeparam name="TMessageType">The messageType enum</typeparam>
    public class MessageReceivedEventArgs<TMessageType> : EventArgs where TMessageType : System.Enum
    {
        public TMessageType MessageType { get; set; }
        public byte[] Message { get; set; }

    }
}