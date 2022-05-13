using System;
using System.Net;
using System.Net.Sockets;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class UdpSocketConnectionController<TMessageType> : IDisposable where TMessageType : System.Enum
    {
        private readonly ISocketConnection _socket;
        private readonly IPEndPoint _targetEndPoint;
        private readonly int _bufferSize;
        private bool _isDisposed;
        private PacketProtocol<TMessageType> _packetProtocol;

        public bool IsConnected { get; private set; }
        public event EventHandler<MessageReceivedEventArgs<TMessageType>> MessageReceived;
        

        public UdpSocketConnectionController(ISocketConnection socket, IPEndPoint targetEndPoint, int bufferSize)
        {
            _socket = socket;
            _targetEndPoint = targetEndPoint;
            _bufferSize = bufferSize;
        }

        public void Start()
        {
            _packetProtocol = new PacketProtocol<TMessageType>(_bufferSize);
            _packetProtocol.MessageArrived = (MessageType, data) => OnMessageReceived(MessageType, data);
            IsConnected = true;
           
            StartReceive();
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
                _socket.BeginSendTo(data, 0, data.Length, 0, _targetEndPoint, SendCallback,data);
            }
            catch (SocketException e)
            {
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Complete sending the data to the remote device.  
                int bytesSent = _socket.EndSend(ar);
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine("Failed to send data as socket was disposed.");
            }
            catch (SocketException e)
            {
            }
        }


        private void StartReceive()
        {
            byte[] buffer = new byte[_bufferSize];
            EndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                _socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref tempRemoteEP, ReceiveCallback,
                    buffer);
            }
            catch (SocketException)
            {
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            EndPoint source = new IPEndPoint(0, 0);
            int bytesRead = 0;
            try
            {
                bytesRead = _socket.EndReceiveFrom(ar, ref source);
            }
            catch (SocketException)
            {
                return;
            }

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
                    _packetProtocol = new PacketProtocol<TMessageType>(_bufferSize);
                    _packetProtocol.MessageArrived = (MessageType, data) => OnMessageReceived(MessageType, data);
                }
            }

            // Start receiving more data
            StartReceive();
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
            IsConnected = false;
        }
    }
}
