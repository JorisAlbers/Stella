using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class UdpSocketConnectionController<TMessageType> : IDisposable where TMessageType : System.Enum
    {
        private ISocketConnection _socketConnection;
        private readonly IPEndPoint _targetEndPoint;
        private bool _isDisposed;
        
        public UdpSocketConnectionController(ISocketConnection socket, IPEndPoint targetEndPoint)
        {
            _socketConnection = socket;
            _targetEndPoint = targetEndPoint;
        }

        public void Start()
        {
            byte[] buffer = new byte[PacketProtocol<TMessageType>.BUFFER_SIZE];
            EndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            _socketConnection.BeginReceiveFrom(buffer, 0, 100, SocketFlags.None, ref tempRemoteEP, ReceiveCallback, buffer);
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
                _socketConnection.SendTo(data, 0, data.Length, 0, _targetEndPoint);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to send data over UDP, {e.Message}");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            // this is what had been passed into BeginReceive as the second parameter:
            byte[] receivedBuffer = ar.AsyncState as byte[];
            // points towards whoever had sent the message:
            EndPoint source = new IPEndPoint(0, 0);
            // get the actual message and fill out the source:
            int received = _socketConnection.EndReceiveFrom(ar, ref source);

            // do what you'd like with `message` here:
            // OnMessageReceived(); TODO
            // schedule the next receive operation once reading is done:
            byte[] buffer = new byte[PacketProtocol<TMessageType>.BUFFER_SIZE];
            _socketConnection.BeginReceiveFrom(buffer, 0, 100, SocketFlags.None, ref source, ReceiveCallback, buffer);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _socketConnection.Disconnect(false);
            _socketConnection.Dispose();
            _socketConnection.Close();
        }

        public static ISocketConnection CreateSocket(IPEndPoint localEndPoint)
        {
            SocketConnection socket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEndPoint);
            return socket;
        }
    }
}
