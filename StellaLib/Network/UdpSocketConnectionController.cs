using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class UdpSocketConnectionController
    {
        private ISocketConnection _socketConnection;
        private readonly IPEndPoint _targetEndPoint;

        public UdpSocketConnectionController(ISocketConnection socket, IPEndPoint targetEndPoint)
        {
            _socketConnection = socket;
            _targetEndPoint = targetEndPoint;
        }

        public void Start()
        {
            byte[] buffer = new byte[PacketProtocol<MessageType>.BUFFER_SIZE];
            EndPoint tempRemoteEP = new IPEndPoint(IPAddress.Any, 0);
            _socketConnection.BeginReceiveFrom(buffer, 0, 100, SocketFlags.None, ref tempRemoteEP, ReceiveCallback, buffer);
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
            byte[] buffer = new byte[PacketProtocol<MessageType>.BUFFER_SIZE];
            _socketConnection.BeginReceiveFrom(buffer, 0, 100, SocketFlags.None, ref source, ReceiveCallback, buffer);
        }

        public static ISocketConnection CreateSocket(IPEndPoint localEndPoint)
        {
            SocketConnection socket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEndPoint);
            return socket;
        }

    }
}
