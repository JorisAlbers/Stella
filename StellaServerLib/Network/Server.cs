using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol.Animation;

namespace StellaServerLib.Network
{
    public class Server : IServer, IDisposable
    {
        /// <summary> The size of the package protocol UDP buffer </summary>
        private const int UDP_BUFFER_SIZE = 60_000; // The maximum UDP package size is 65,507 bytes.

        private List<Client> _newConnections;
        private ConcurrentDictionary<int,Client> _clients;

        private IPEndPoint _udpBroadcastEndpoint;
        private IPEndPoint _udpOutputEndPoint;

        private int _port;
        private int _udpPort;
        private int _remoteUdpPort;

        private bool _isShuttingDown = false;
        private readonly object _isShuttingDownLock = new object();

        private ISocketConnection _udpSocketConnection;

        public event EventHandler<ClientStatusChangedEventArgs> ClientChanged;

        public Server()
        {
            _newConnections =  new List<Client>();
            _clients = new ConcurrentDictionary<int, Client>();
        }

        public void Start(string ip, int port, int udpPort, int remoteUdpPort, SocketConnectionCreator socketConnectionCreator)
        {
            Console.Out.WriteLine($"Starting server on {port}");

            IPAddress ipAddress = IPAddress.Parse(ip);
            _udpBroadcastEndpoint = new IPEndPoint(ipAddress, port);
            _udpOutputEndPoint = new IPEndPoint(ipAddress, udpPort);
            _port = port;
            _udpPort = udpPort;
            _remoteUdpPort = remoteUdpPort;

            // Create an UDP socket.
            _udpSocketConnection = UdpSocketConnectionController<MessageType>.CreateSocket(_udpOutputEndPoint);

            // TODO create UDP socket to listen for broadcasts
            var broadCastSocket = socketConnectionCreator.CreateForBroadcast(_udpBroadcastEndpoint);

            var broadCastConnection =
                new UdpSocketConnectionController<MessageType>(broadCastSocket, _udpBroadcastEndpoint, UDP_BUFFER_SIZE);

        }

        public void SendToClient(int clientId, MessageType messageType)
        {
            SendToClient(clientId,messageType,new byte[0]);
        }

        public void SendToClient(int clientId, FrameWithoutDelta frame)
        {
            byte[][] packages = FrameProtocol.SerializeFrame(frame, UDP_BUFFER_SIZE); // TODO move the switch between UDP and TCP send from Client to here.
            for (int i = 0; i < packages.Length; i++)
            {
                SendToClient(clientId, MessageType.AnimationRenderFrame, packages[i]);
            }
        }

        private void SendToClient(int clientId, MessageType messageType, byte[] data)
        {
            try
            {
                if (_clients.TryGetValue(clientId, out Client client))
                {
                    if (messageType == MessageType.AnimationRenderFrame)
                    {
                        client.SendUdp(messageType, data);
                    }
                    else
                    {
                        client.SendTcp(messageType, data);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.Out.WriteLine($"Failed to send message to client, {e.SocketErrorCode}");
            }
        }

        // TODO handle UDP broadcast message received:
        // TODO create new potential client, send init, wait for confirmation, add to connected clients
        /*private void AcceptCallback(IAsyncResult ar) 
        {
            Console.Out.WriteLine("Received accept callback");
            // Get the socket that handles the client request.  
            ISocketConnection listener = (ISocketConnection) ar.AsyncState;  

            lock(_isShuttingDownLock)
            {
                if(_isShuttingDown)
                {
                    Console.WriteLine("Ignored new client callback as the server is shutting down.");
                    return;
                }
            }

            // Start an asynchronous socket to listen for connections.  


            // Handle the new connection
            ISocketConnection handler = listener.EndAccept(ar);

            // Create a new client.
            // For this, we need a TCP and an UDP connection.
            // Create TCP connection
            SocketConnectionController<MessageType> socketConnectionController = new SocketConnectionController<MessageType>(handler, TCP_BUFFER_SIZE);
            //  Create UDP connection
            IPEndPoint udpRemoteEndPoint = new IPEndPoint(((IPEndPoint)handler.RemoteEndPoint).Address, _remoteUdpPort);

            UdpSocketConnectionController<MessageType> udpSocketConnectionController = new UdpSocketConnectionController<MessageType>(_udpSocketConnection, udpRemoteEndPoint, UDP_BUFFER_SIZE);
            Client client = new Client(socketConnectionController, udpSocketConnectionController);
            client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;

            // As we do not yet know which client this is, add him to the list of new connection.
            lock (_newConnections)
            {
                _newConnections.Add(client);
            }

            client.Start();
        }*/

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            Client client = (Client)sender;
            switch(e.MessageType)
            {
                case MessageType.Init:
                    // The client sends its ID
                    ParseInitMessage(client, e.Message);
                    break;
                default:
                    Console.WriteLine($"Message type {e.MessageType} is not supported by the server");
                    break;
            }
        }

        private void Client_Disconnected(object sender, SocketException exception)
        {
            Client client = (Client)sender;
            if(client.ID == -1)
            {
                Console.WriteLine($"New client disconnected");
            }
            else
            {
                Console.WriteLine($"Client {client.ID} disconnected, {exception.SocketErrorCode}"); // TODO add intended disconnect, not just on SocketException
            }
            ClientChanged?.Invoke(this, new ClientStatusChangedEventArgs(client.ID, ClientStatus.Disconnected));
            DisposeClient(client);
        }

        private void ParseInitMessage(Client client, byte[] message)
        {
            int id = BitConverter.ToInt32(message,0);

            if(client.ID != -1)
            {
                Console.WriteLine($"INIT is invalid. Client with ID {client.ID} wants to set his ID to {id}, but he already has an ID.");
                return;
            }

            client.ID = id;
            string logMessage = $"Client has initialized itself with id {id}";

            _clients.AddOrUpdate(id, client, (key, oldValue) =>
            {
                logMessage = $"A client with ID {id} already exists. Replacing the existing one.";
                DisposeClient(_clients[id]);
                return client;
            });

            Console.Out.WriteLine(logMessage);
            ClientChanged?.Invoke(this, new ClientStatusChangedEventArgs(id ,ClientStatus.Connected));

            lock(_newConnections)
            {
                _newConnections.Remove(client);
            }
        }

        public void Dispose()
        {
            lock(_isShuttingDownLock)
            {
                _isShuttingDown = true;
            }

            foreach(Client client in _newConnections)
            {
                DisposeClient(client);
            }

            foreach(Client client in _clients.Values)
            {
                DisposeClient(client);
            }

            try
            {
                // TODO dispose UDP client
            }
            catch (SocketException) { }
            try
            {
                _udpSocketConnection.Shutdown(SocketShutdown.Receive);
                _udpSocketConnection.Dispose();
                _udpSocketConnection.Close();

            }
            catch (SocketException){}

        }

        private void DisposeClient(Client client)
        {
            _clients.TryRemove(client.ID, out _);
            client.MessageReceived -= Client_MessageReceived;
            client.Disconnect -= Client_Disconnected;
            client.Dispose();
        }
    }
}