using System;
using StellaLib.Network;

namespace StellaLib.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageType MessageType {get;set;}
        public byte[] Message {get;set;}
        
    }
}