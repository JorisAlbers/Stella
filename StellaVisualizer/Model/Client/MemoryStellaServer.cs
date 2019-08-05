using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StellaClientLib.Network;
using StellaLib.Animation;
using StellaLib.Network;

namespace StellaTestSuite.Model.Client
{
    public class MemoryStellaServer : IStellaServer
    {
        /// <summary>
        /// Fired when the client is sending a message
        /// </summary>
        public event EventHandler<MessageType> MessageSend;

        public event EventHandler<FrameWithoutDelta> RenderFrameReceived;

        /// <summary>
        /// Fakes an incoming message by firing the RenderFrameReceived event
        /// </summary>
        /// <param name="frame"></param>
        public void OnRenderFrameReceived(FrameWithoutDelta frame)
        {
            var eventHandler = RenderFrameReceived;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,frame);
            }
        }

        public void Dispose()
        {
            ;
        }

        public void Send(MessageType type, byte[] message)
        {
            var eventHandler = MessageSend;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,type);
            }
        }

        public void Start(IPEndPoint serverAdress, int udpPort, int ID)
        {
            ;
        }



    }
}
