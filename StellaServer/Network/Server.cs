using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using StellaLib.Network;

namespace StellaServer.Network
{
    public class Server : IDisposable
    {
        private List<Client> _newConnections;
        private Dictionary<string,Client> _clients;

        private int _port;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private Socket _listenerSocket;

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
            _listenerSocket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
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

        public void SendMessageToClient(string clientID, string message)
        {
            SendMessageToClient(clientID, MessageType.Standard, message);
        }

        private void SendMessageToClient(string clientID, MessageType messageType, string message)
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
            Socket handler = listener.EndAccept(ar); 

            // Create a new client.
            Client client = new Client(new SocketConnection(handler));
            client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;
            client.Start();
            
            // As we do not now wich client this is, add him to the list of new connection.
            lock(_newConnections)
            {
                _newConnections.Add(client);
            }
            
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

        private void ParseInitMessage(Client client, string message)
        {
            // The message should be an identifier.
            lock(_clients)
            {
                if(client.ID != null && client.ID != message)
                {
                    Console.WriteLine($"INIT is invalid. Client with ID {client.ID} wants to set his ID to {message}, but he already has an ID.");
                    return;
                }

                client.ID = message;
                if(_clients.ContainsKey(message))
                {
                    Console.WriteLine($"A client with ID {message} already exists. Replacing the existing one.");
                    DisposeClient(_clients[message]);
                    _clients[message] = client;
                }
                else
                {
                    Console.WriteLine($"Client has initialized itself with id {message}");
                    _clients.Add(message,client);
                }
            }
           
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