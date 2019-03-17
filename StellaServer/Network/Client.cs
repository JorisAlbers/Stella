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
        private SocketConnection _socketConnection {get;set;}
        public string ID {get;set;}
        public event EventHandler Disconnect;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;


        public Client(SocketConnection socketConnection)
        {
            _socketConnection = socketConnection;
            _socketConnection.Disconnect += OnDisconnect;
            _socketConnection.MessageReceived += OnMessageReceived;
        }

        public void Start()
        {
            _socketConnection.Start();
        }

        public void Send(MessageType type, byte[] message)
        {
            _socketConnection.Send(type,message);
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

        protected virtual void OnDisconnect(object sender, EventArgs eventAgs)
        {
            // Bubble the event. Add reference to this object.
            EventHandler handler = Disconnect;
            if (handler != null)
            {
                handler(this, eventAgs);
            }
        }

        public void Dispose()
        {
            _socketConnection.MessageReceived -= OnMessageReceived;
            _socketConnection.Disconnect -= OnDisconnect;
            _socketConnection.Dispose();
        }
    }
}