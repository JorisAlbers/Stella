﻿using System;
using StellaLib.Animation;
using StellaLib.Network;
using StellaServerLib.Network;

namespace StellaVisualizer.Model.Server
{
    public class MemoryServer : IServer
    {
        /// <summary>
        /// The server is sending a message to a client
        /// </summary>
        public event EventHandler<MessageSendEventArgs> FrameSend;

        public void Start(int broadcastPort, int udpPort, int remoteUdpPort, SocketConnectionCreator _)
        {
            ;
        }

        public void SendToClient(int clientId, MessageType messageType)
        {
            throw new NotImplementedException();
        }

        public void SendToClient(int clientId, FrameWithoutDelta frame)
        {
            var eventHandler = FrameSend;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,new MessageSendEventArgs
                {
                    ID = clientId,
                    frame = frame
                });
            }
        }

        public event EventHandler<ClientStatusChangedEventArgs> ClientChanged;

        public void Dispose()
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
