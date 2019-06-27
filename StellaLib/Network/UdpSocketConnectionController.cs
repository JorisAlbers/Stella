using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class UdpSocketConnectionController<TMessageType> : IDisposable where TMessageType : System.Enum
    {
        private readonly ISocketConnection _socket;
        private readonly IPEndPoint _targetEndPoint;
        private bool _isDisposed;
        private PacketProtocol<TMessageType> _packetProtocol;

        public event EventHandler<MessageReceivedEventArgs<TMessageType>> MessageReceived;


        public UdpSocketConnectionController(ISocketConnection socket, IPEndPoint targetEndPoint)
        {
            _socket = socket;
            _targetEndPoint = targetEndPoint;
        }

        public void Start()
        {
            _packetProtocol = new PacketProtocol<TMessageType>();
            _packetProtocol.MessageArrived = (MessageType, data) => OnMessageReceived(MessageType, data);
            
            byte[] buffer = new byte[PacketProtocol<TMessageType>.BUFFER_SIZE];
            EndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            _socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref tempRemoteEP, ReceiveCallback, buffer);
        }

        public void Send(TMessageType messageType, byte[] message)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("SocketConnectionController has been disposed");
            }

            byte[] data = PacketProtocol<TMessageType>.WrapMessage(messageType, message);

            // Begin sending the data to the remote device.  
            try
            {
                _socket.SendTo(data, 0, data.Length, 0, _targetEndPoint);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to send data over UDP, {e.Message}");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            EndPoint source = new IPEndPoint(0, 0);

            int bytesRead = _socket.EndReceiveFrom(ar, ref source);
            // Parse the message
            if (source.Equals(_targetEndPoint) && bytesRead > 0)
            {
                try
                {
                    byte[] b = (byte[])ar.AsyncState;
                    _packetProtocol.DataReceived(b, bytesRead);
                }
                catch (ProtocolViolationException e)
                {
                    _packetProtocol.MessageArrived = null;
                    _packetProtocol = new PacketProtocol<TMessageType>();
                    _packetProtocol.MessageArrived = (MessageType, data) => OnMessageReceived(MessageType, data);
                }
            }

            // Start receiving more data
            byte[] buffer = new byte[PacketProtocol<TMessageType>.BUFFER_SIZE];
            _socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref source, ReceiveCallback, buffer);
            
        }

        protected virtual void OnMessageReceived(TMessageType type, byte[] bytes)
        {
            EventHandler<MessageReceivedEventArgs<TMessageType>> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageReceivedEventArgs<TMessageType>()
                {
                    MessageType = type,
                    Message = bytes
                });
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _socket.Disconnect(false);
            _socket.Dispose();
            _socket.Close();
        }

        public static ISocketConnection CreateSocket(IPEndPoint localEndPoint)
        {
            SocketConnection socket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = false;
            socket.Bind(localEndPoint);
            return socket;
        }
    }
}
