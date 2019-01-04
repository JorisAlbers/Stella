using System;

namespace StellaLib.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageType MessageType {get;set;}
        public string Message {get;set;}
        
    }
}