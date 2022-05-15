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
        private byte _STELLA_PROTOCOL_KEY = 73;
        private readonly SocketConnectionCreator _socketConnectionCreator;
        private readonly int _port;
        private ISocketConnection _socket;
        private const int UDP_BUFFER_SIZE = 60_000; // The maximum UDP package size is 65,507 bytes.

        private readonly Dictionary<IPEndPoint, PacketProtocol<MessageType>> _packageProtocolPerClient;

        public event EventHandler<IPEndPoint> NewClientRegistered;


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
                    _packageProtocolPerClient.Add(ipSource, packetProtocol);
                }


                try
                {
                    byte[] b = (byte[])ar.AsyncState;
                    if (packetProtocol.DataReceived(b, bytesRead, out Message<MessageType> message))
                    {
                        ParseMessage(message, ipSource);
                    }

                }
                catch (ProtocolViolationException e)
                {
                    Console.Error.WriteLine($"Failed to parse message from {ipSource}, ProtocolViolationException");
                    _packageProtocolPerClient.Remove(ipSource);
                }
            }

            // Start receiving more data
            StartReceive();
        }

        private void ParseMessage(Message<MessageType> message, IPEndPoint source)
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
                    ParseConnectionRequest(message.Data, source);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
                // TODO cleanup old packetProtocols.
            }
        }

        private void ParseConnectionRequest(byte[] messageData, IPEndPoint ipEndPoint)
        {
            ConnectionRequestMessage message = ConnectionRequestProtocol.Deserialize(messageData, 0);
            Console.Out.WriteLine($"Received connection request message with key {message.Key} , version {message.Version}");

            if (message.Key != _STELLA_PROTOCOL_KEY)
            {
                Console.Error.WriteLine($"Failed to parse {nameof(ConnectionRequestProtocol)}, the stella protocol key is incorrect, should be {_STELLA_PROTOCOL_KEY}");
                return;
            }

            if (message.Version > 1)
            {
                Console.Error.WriteLine($"Failed to parse {nameof(ConnectionRequestProtocol)}, the protocol version is unknown. Expected 1, got {message.Version}");
                return;
            }

            NewClientRegistered?.Invoke(this,ipEndPoint);
        }

    }
}
