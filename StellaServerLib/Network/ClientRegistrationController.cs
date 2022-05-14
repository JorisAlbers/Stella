using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaServerLib.Network
{
    public class ClientRegistrationController
    {
        private readonly SocketConnectionCreator _socketConnectionCreator;
        private readonly int _port;
        private ISocketConnection _socket;
        private const int UDP_BUFFER_SIZE = 60_000; // The maximum UDP package size is 65,507 bytes.

        private readonly Dictionary<IPEndPoint, PacketProtocol<MessageType>> _packageProtocolPerClient;

        public ClientRegistrationController(SocketConnectionCreator socketConnectionCreator, int port)
        {
            _socketConnectionCreator = socketConnectionCreator;
            _port = port;
            _packageProtocolPerClient = new Dictionary<IPEndPoint, PacketProtocol<MessageType>>();
        }

        public void Start()
        {
            _socket =  _socketConnectionCreator.CreateForBroadcast(_port);
            StartReceive();
        }

        public void Stop()
        {
            try
            {
                _socket.Disconnect(false);
                _socket.Dispose();
            }
            catch {  }
        }

        private void StartReceive()
        {
            byte[] buffer = new byte[UDP_BUFFER_SIZE];
            EndPoint tempRemoteEp = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                _socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref tempRemoteEp, ReceiveCallback,
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
            if (bytesRead > 0) // TODO check if disposed
            {
                IPEndPoint ipSource = (IPEndPoint)source;

                PacketProtocol<MessageType> packetProtocol;

                if (!_packageProtocolPerClient.TryGetValue(ipSource, out packetProtocol))
                {
                    packetProtocol = new PacketProtocol<MessageType>(UDP_BUFFER_SIZE);
                }

                
                try
                {
                    byte[] b = (byte[])ar.AsyncState;
                    if (packetProtocol.DataReceived(b, bytesRead, out Message<MessageType> message))
                    {
                        ParseMessage(message);
                    };
                }
                catch (ProtocolViolationException e)
                {
                    // TODO
                    //packetProtocol.Reset();
                }
            }

            // Start receiving more data
            StartReceive();
        }

        private void ParseMessage(Message<MessageType> message)
        {
            switch (message.MessageType)
            {
                case MessageType.Unknown:
                case MessageType.Init:
                case MessageType.Standard:
                case MessageType.AnimationRenderFrame:
                    Console.Error.WriteLine($"Message type {message.MessageType} not supported by {this.GetType().Name}");
                    break;
                case MessageType.ConnectionRequest:
                    ParseConnectionRequest(message.Data);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ParseConnectionRequest(byte[] messageData)
        {
            throw new NotImplementedException();
        }
    }
}
