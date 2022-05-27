using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Network
{
    public class Server : IServer, IDisposable
    {
        /// <summary> The size of the package protocol UDP buffer </summary>
        private const int UDP_BUFFER_SIZE = 1024 * 2 * 7 - 8; 

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
            _clientMappings = clientMappings;
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
            byte[][] packages = FrameProtocol.SerializeFrame(frame, UDP_BUFFER_SIZE);
            SendToClient(clientId, MessageType.FrameHeader, packages[0]);
            for (int i = 1; i < packages.Length; i++)
            {
                SendToClient(clientId, MessageType.FrameSection, packages[i]);
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
            Client client = _clients.GetOrAdd(ipAndIndex.index, (index) => CreateNewClient(ipAndIndex));
          
            // Send init message
            // TODO move initializing client to registrationController
            byte[] data = InitProtocol.Serialize(_clientMappings.First(x => x.Index == ipAndIndex.index).Pixels, 255);
            client.SendUdp(MessageType.Init, data);
            ClientChanged?.Invoke(this, new ClientStatusChangedEventArgs(ipAndIndex.index, ClientStatus.Connected));
        }

        private Client CreateNewClient((IPEndPoint ip, int index) ipAndIndex)
        {
            // Create a new client.
            UdpSocketConnectionController<MessageType> udpSocketConnectionController = new UdpSocketConnectionController<MessageType>(_udpSocketConnection, ipAndIndex.ip, UDP_BUFFER_SIZE);
            Client client = new Client(udpSocketConnectionController);

            // TODO fix client.Disconnect += Client_Disconnected;
            client.MessageReceived += Client_MessageReceived;
            client.Start();

            return client;
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