using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol.Animation;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Network
{
    public class Server : IServer, IDisposable
    {
        /// <summary> The size of the package protocol UDP buffer </summary>
        private const int UDP_BUFFER_SIZE = 60_000; // The maximum UDP package size is 65,507 bytes.

        private List<Client> _newConnections;
        private ConcurrentDictionary<int,Client> _clients;

        private IPEndPoint _udpOutputEndPoint;

        private int _port;
        private int _udpPort;
        private int _remoteUdpPort;
        private List<ClientMapping> _clientMappings;

        private bool _isShuttingDown = false;
        private readonly object _isShuttingDownLock = new object();

        private ISocketConnection _udpSocketConnection;
        private ClientRegistrationController _clientRegistrationController;

        public event EventHandler<ClientStatusChangedEventArgs> ClientChanged;

        public Server()
        {
            _newConnections =  new List<Client>();
            _clients = new ConcurrentDictionary<int, Client>();
        }

        public void Start(int broadcastPort, int udpPort, int remoteUdpPort,
            SocketConnectionCreator socketConnectionCreator, List<ClientMapping> clientMappings)
        {
            string ip = GetIp();

            Console.Out.WriteLine($"Starting server on {ip} {broadcastPort}");

            IPAddress ipAddress = IPAddress.Parse(ip);
            _udpOutputEndPoint = new IPEndPoint(ipAddress, udpPort);
            _port = broadcastPort;
            _udpPort = udpPort;
            _remoteUdpPort = remoteUdpPort;

            // Create an UDP socket for output.
            _udpSocketConnection = socketConnectionCreator.Create(_udpOutputEndPoint);

            _clientRegistrationController = new ClientRegistrationController(socketConnectionCreator, broadcastPort, clientMappings);
            _clientRegistrationController.NewClientRegistered += ClientRegistrationController_OnNewClientRegistered;
            _clientRegistrationController.Start();
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
                    client.SendUdp(messageType, data);
                }
            }
            catch (SocketException e)
            {
                Console.Out.WriteLine($"Failed to send message to client, {e.SocketErrorCode}");
            }
        }

        private void ClientRegistrationController_OnNewClientRegistered(object sender, (IPEndPoint ip, int index) ipAndIndex)
        {
            if (_clients.ContainsKey(ipAndIndex.index))
            {
                Console.WriteLine($"Client {ipAndIndex.index} is already registered.");
                return;
            }

            // Create a new client.
            UdpSocketConnectionController<MessageType> udpSocketConnectionController = new UdpSocketConnectionController<MessageType>(_udpSocketConnection, ipAndIndex.ip, UDP_BUFFER_SIZE);
            Client client = new Client(udpSocketConnectionController);

            bool newClient = true;
            _clients.AddOrUpdate(ipAndIndex.index, client, (key, oldValue) =>
            {
                newClient = false;
                return oldValue;
            });

            if (!newClient)
            {
                return;
            }


            client.MessageReceived += Client_MessageReceived;
            // TODO fix client.Disconnect += Client_Disconnected;

            // As we do not yet know which client this is, add him to the list of new connection.
            client.Start();
            client.SendUdp(MessageType.Init, new byte[]{10}); // TODO create init protocol and message. TODO move to registrationController
            
            ClientChanged?.Invoke(this, new ClientStatusChangedEventArgs(ipAndIndex.index, ClientStatus.Connected));
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            Client client = (Client)sender;
            switch(e.MessageType)
            {
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

        private string GetIp()
        {
            string localIP;
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            localIP = endPoint.Address.ToString();

            return localIP;
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