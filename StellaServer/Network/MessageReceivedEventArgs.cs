using System;
using StellaLib.Network;

namespace StellaServer.Network
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageType MessageType {get;set;}
        public string Message {get;set;}
        
    }
}