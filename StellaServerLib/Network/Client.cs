using System;
using System.Net.Sockets;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public class Client : IDisposable
    {
        private readonly SocketConnectionController<MessageType> _socketConnectionController;
        private readonly UdpSocketConnectionController<MessageType> _udpSocketConnectionController;
        public int ID { get; set; } = -1;
        public event EventHandler<SocketException> Disconnect;
        public event EventHandler<MessageReceivedEventArgs<MessageType>> MessageReceived;


        public Client(SocketConnectionController<MessageType> socketConnectionController, UdpSocketConnectionController<MessageType> udpSocketConnectionController)
        {
            // TCP socketConnectionController
            _socketConnectionController = socketConnectionController;
            _udpSocketConnectionController = udpSocketConnectionController;
            _socketConnectionController.Disconnect += OnDisconnect;
            _socketConnectionController.MessageReceived += OnMessageReceived;

            // UDP socketConnectionController
            _udpSocketConnectionController = udpSocketConnectionController;
            _udpSocketConnectionController.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            _socketConnectionController.Start();
            _udpSocketConnectionController.Start();
        }

        public void Send(MessageType type, byte[] message)
        {
            if (type == MessageType.Animation_PrepareFrame || type == MessageType.Animation_RenderFrame)
            {
                // Send via UDP
                _udpSocketConnectionController.Send(type,message);
                return;
            }

            // Send via TCP
            _socketConnectionController.Send(type, message);
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
            _socketConnectionController.MessageReceived -= OnMessageReceived;
            _socketConnectionController.Disconnect -= OnDisconnect;
            _socketConnectionController.Dispose();
        }
    }
}