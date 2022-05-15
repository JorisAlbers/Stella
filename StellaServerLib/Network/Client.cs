using System;
using System.Net.Sockets;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public class Client : IDisposable
    {
        private readonly UdpSocketConnectionController<MessageType> _udpSocketConnectionController;
        public int ID { get; set; } = -1;
        public event EventHandler<SocketException> Disconnect;
        public event EventHandler<MessageReceivedEventArgs<MessageType>> MessageReceived;


        public Client(UdpSocketConnectionController<MessageType> udpSocketConnectionController)
        {
            // UDP socketConnectionController
            _udpSocketConnectionController = udpSocketConnectionController;
            _udpSocketConnectionController.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            _udpSocketConnectionController.Start();
        }

        public void SendUdp(MessageType type, byte[] message)
        {
            _udpSocketConnectionController.Send(type, message);
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
            _udpSocketConnectionController.MessageReceived -= MessageReceived;
            _udpSocketConnectionController.Dispose();
        }
    }
}