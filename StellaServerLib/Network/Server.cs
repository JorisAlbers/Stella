using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaServerLib.Network
{
    public class Server : IServer, IDisposable
    {
        private List<Client> _newConnections;
        private Dictionary<int,Client> _clients;

        private int _port;
        private IPAddress _ip;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private ISocketConnection _listenerSocket;

        public event EventHandler<AnimationRequestEventArgs> AnimationRequestReceived;

        public Server(string ip, int port)
        {
            _ip = IPAddress.Parse(ip);
            _port = port;
            _newConnections =  new List<Client>();
            _clients = new Dictionary<int, Client>();
        }

        public void Start()
        {
            IPEndPoint localEndPoint = new IPEndPoint(_ip, _port);

            Console.Out.WriteLine($"Starting server on {_port}");
            // Create a TCP/IP socket.  
            _listenerSocket = new SocketConnection(_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject with ISocketConnection
            // Bind the socket to the local endpoint and listen for incoming connections.  

            _listenerSocket.Bind(localEndPoint);  
            _listenerSocket.Listen(100);  
        
            // Start an asynchronous socket to listen for connections.  
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket );  
        }

        public int[] ConnectedClients
        {
            get
            {
                lock(_clients)
                {
                    return _clients.Keys.ToArray();
                }
            }
        }

        public int NewConnectionsCount
        {
            get
            {
                lock(_newConnections)
                {
                    return _newConnections.Count;
                }
            }
        }

        public void SendMessageToClient(int clientID, MessageType messageType, byte[] message)
        {
            lock(_clients)
            {
                _clients[clientID].Send(messageType,message);
            }
        }

        private void AcceptCallback(IAsyncResult ar) 
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
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);

            // Handle the new connection
            ISocketConnection handler = listener.EndAccept(ar);
            
            // Create a new client.
            Client client = new Client(new SocketConnectionController<MessageType>(handler));
            client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;

            // As we do not yet know which client this is, add him to the list of new connection.
            lock (_newConnections)
            {
                _newConnections.Add(client);
            }

            client.Start();
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            Client client = (Client)sender;
            switch(e.MessageType)
            {
                case MessageType.Init:
                    // The client sends its ID
                    ParseInitMessage(client, e.Message);
                    break;
                case MessageType.TimeSync:
                    // The client wants to sync the time
                    ParseTimeSyncMessage(client, e.Message);
                    break;
                case MessageType.Animation_Request:
                    // The client request the next n frames
                    OnAnimationRequestReceived(client.ID,e.Message);
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

            DisposeClient(client);
        }

        private void ParseInitMessage(Client client, byte[] message)
        {
            int id = BitConverter.ToInt32(message,0);
            
            lock(_clients)
            {
                if(client.ID != -1)
                {
                    Console.WriteLine($"INIT is invalid. Client with ID {client.ID} wants to set his ID to {id}, but he already has an ID.");
                    return;
                }

                client.ID = id;
                if(_clients.ContainsKey(id))
                {
                    Console.WriteLine($"A client with ID {id} already exists. Replacing the existing one.");
                    DisposeClient(_clients[id]);
                    _clients[id] = client;
                }
                else
                {
                    Console.WriteLine($"Client has initialized itself with id {id}");
                    _clients.Add(id,client);
                }
            }
           
            lock(_newConnections)
            {
                _newConnections.Remove(client);
            }
        }

        private void ParseTimeSyncMessage(Client client, byte[] message)
        {
            Console.WriteLine($"Synchronizing time with client {client.ID} ");
            client.Send(MessageType.TimeSync,TimeSyncProtocol.CreateMessage(DateTime.Now,message));
        }

        private void OnAnimationRequestReceived(int clientID, byte[] message)
        {
            int startIndex, count;
            AnimationRequestProtocol.ParseRequest(message,out startIndex,out count);

            Console.WriteLine($"Client {clientID} has requested {count} frames starting from index {startIndex}");
            EventHandler<AnimationRequestEventArgs> eventHandler = AnimationRequestReceived;
            if (eventHandler != null)
            {
                eventHandler(this,new AnimationRequestEventArgs(clientID,startIndex,count));
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
                _listenerSocket.Shutdown(SocketShutdown.Receive);

            }
            catch(SocketException e)
            {
                Console.WriteLine("Listening socket was already disconnected.");
            }
            _listenerSocket.Dispose();
            _listenerSocket.Close();
        }

        private void DisposeClient(Client client)
        {
            client.MessageReceived -= Client_MessageReceived;
            client.Disconnect -= Client_Disconnected;
            client.Dispose();
        }
    }
}