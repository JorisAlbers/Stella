using System;
using System.Net.Sockets;
using StellaLib.Network;

namespace StellaServerAPI
{
    public class Client : IDisposable
    {
        private SocketConnectionController<MessageType> SocketConnectionController {get;set;}
        public int ID { get; set; } = -1;
        public event EventHandler<SocketException> Disconnect;
        public event EventHandler<MessageReceivedEventArgs<MessageType>> MessageReceived;


        public Client(SocketConnectionController<MessageType> socketConnectionController)
        {
            SocketConnectionController =socketConnectionController;
            SocketConnectionController.Disconnect += OnDisconnect;
            SocketConnectionController.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            SocketConnectionController.Start();
        }

        public void Send(MessageType type, byte[] message)
        {
            SocketConnectionController.Send(type,message);
        }

        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs<MessageType> eventArgs)
        {
            // Bubble the event. Add reference to this object.
            EventHandler<MessageReceivedEventArgs<MessageType>> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        protected virtual void OnDisconnect(object sender, SocketException exception)
        {
            // Bubble the event. Add reference to this object.
            EventHandler<SocketException> handler = Disconnect;
            if (handler != null)
            {
                handler(this, exception);
            }
        }

        public void Dispose()
        {
            SocketConnectionController.MessageReceived -= OnMessageReceived;
            SocketConnectionController.Disconnect -= OnDisconnect;
            SocketConnectionController.Dispose();
        }
    }
}