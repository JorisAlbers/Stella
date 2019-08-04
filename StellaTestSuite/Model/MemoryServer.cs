using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaLib.Network;
using StellaServerLib.Network;

namespace StellaTestSuite.Model
{
    public class MemoryServer : IServer
    {
        /// <summary>
        /// The server is sending a message to a client
        /// </summary>
        public event EventHandler<MessageSendEventArgs> MessageSend;

        public void SendToClient(int clientId, MessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void SendToClient(int clientId, FrameWithoutDelta frame)
        {
            var eventHandler = MessageSend;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,new MessageSendEventArgs
                {
                    ID = clientId,
                    frame = frame
                });
            }
        }

        public void Dispose()
        {
            ;
        }

        public void Start()
        {
            ;
        }
    }

    public class MessageSendEventArgs : EventArgs
    {
        public int ID { get; set; }
        public FrameWithoutDelta frame { get; set; }
    }
}
