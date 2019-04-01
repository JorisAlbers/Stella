using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaServer.Network
{
    public class Server : IServer, IDisposable
    {
        private List<Client> _newConnections;
        private Dictionary<string,Client> _clients;

        private int _port;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private ISocketConnection _listenerSocket;

        public event EventHandler<AnimationRequestEventArgs> AnimationRequestReceived;

        public Server(int port)
        {
            _port = port;
            _newConnections =  new List<Client>();
            _clients = new Dictionary<string, Client>();
        }

        public void Start()
        {
            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, _port);  
    
            // Create a TCP/IP socket.  
            _listenerSocket = new SocketConnection(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject with ISocketConnection
            // Bind the socket to the local endpoint and listen for incoming connections.  

            _listenerSocket.Bind(localEndPoint);  
            _listenerSocket.Listen(100);  
        
            // Start an asynchronous socket to listen for connections.  
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket );  
        }

        public string[] ConnectedClients
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

        public void SendMessageToClient(string clientID, MessageType messageType, byte[] message)
        {
            lock(_clients)
            {
                _clients[clientID].Send(messageType,message);
            }
        }

        private void AcceptCallback(IAsyncResult ar) 
        {     
            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;  

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
            ISocketConnection handler = (ISocketConnection) listener.EndAccept(ar);

            // Create a new client.
            Client client = new Client(new SocketConnectionController(handler));
            client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;
            client.Start();
            // As we do not now wich client this is, add him to the list of new connection.
            lock(_newConnections)
            {
                _newConnections.Add(client);
            }

            // Request init from client
            client.Send(MessageType.Init,new byte[0]);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
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

        private void Client_Disconnected(object sender, EventArgs e)
        {
            Client client = (Client)sender;
            if(client.ID == null)
            {
                Console.WriteLine($"New client disconnected");
            }
            else
            {
                Console.WriteLine($"Client {client.ID} disconnected");
            }

            DisposeClient(client);
        }

        private void ParseInitMessage(Client client, byte[] message)
        {
            string id = Encoding.ASCII.GetString(message);
            
            lock(_clients)
            {
                if(client.ID != null && client.ID != id)
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
            client.Send(MessageType.TimeSync,TimeSyncProtocol.CreateMessage(message));
        }

        private void OnAnimationRequestReceived(string clientID, byte[] message)
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