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
            IPEndPoint source = new IPEndPoint(0, 0);
            EndPoint endPoint = (EndPoint)source;;
            int bytesRead = 0;
            try
            {
                bytesRead = _socket.EndReceiveFrom(ar, ref endPoint);
            }
            catch (SocketException)
            {
                return;
            }

            // Parse the message
            if (bytesRead > 0) // TODO check if disposed
            {
                if (!_packageProtocolPerClient.ContainsKey(source))
                {
                    _packageProtocolPerClient.Add(source, new PacketProtocol<MessageType>(UDP_BUFFER_SIZE));
                    // TODO subscribe to message received
                }

                var packageProtocol = _packageProtocolPerClient[source];
                
                try
                {
                    byte[] b = (byte[])ar.AsyncState;
                    packageProtocol.DataReceived(b, bytesRead);
                }
                catch (ProtocolViolationException e)
                {
                    // TODO restore
                    /*packageProtocol.MessageArrived = null;
                    packageProtocol = new PacketProtocol<TMessageType>(_bufferSize);
                    packageProtocol.MessageArrived = (MessageType, data) => OnMessageReceived(MessageType, data);*/
                }
            }

            // Start receiving more data
            StartReceive();
        }
    }
}
