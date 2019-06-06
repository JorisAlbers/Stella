using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;

namespace StellaServer.Network
{
    public class Client : IDisposable
    {
        private SocketConnectionController SocketConnectionController {get;set;}
        public int ID { get; set; } = -1;
        public event EventHandler<SocketException> Disconnect;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        public Client(SocketConnectionController socketConnectionController)
        {
            SocketConnectionController = socketConnectionController;
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

        protected virtual void OnMessageReceived(object sender, MessageReceivedEventArgs eventArgs)
        {
            // Bubble the event. Add reference to this object.
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
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