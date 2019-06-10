using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;

namespace StellaServerAPI
{
    /// <summary>
    /// Connection between this program and API users.
    /// </summary>
    public class APIServer
    {
        private readonly string _ip;
        private readonly int _port;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private ISocketConnection _listenerSocket;

        public List<Client> ConnectedClients { get; set; }
        
        public APIServer(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }

        

        public void Start()
        {
            ConnectedClients = new List<Client>();
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

            Console.Out.WriteLine($"Starting APIServer on {_port}");
            // Create a TCP/IP socket.  
            _listenerSocket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject with ISocketConnection
            // Bind the socket to the local endpoint and listen for incoming connections.  

            _listenerSocket.Bind(localEndPoint);
            _listenerSocket.Listen(100);

            // Start an asynchronous socket to listen for connections.  
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Console.Out.WriteLine("Received accept callback");
            // Get the socket that handles the client request.  
            ISocketConnection listener = (ISocketConnection)ar.AsyncState;

            lock (_isShuttingDownLock)
            {
                if (_isShuttingDown)
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
            //client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;

            ConnectedClients.Add(client);
            client.Start();
        }

        private void Client_Disconnected(object sender, SocketException e)
        {
            Client client = (Client)sender;
            Console.WriteLine($"Client {client.ID} disconnected, {e.SocketErrorCode}"); // TODO add intended disconnect, not just on SocketException
            ConnectedClients.Remove(client);
            DisposeClient(client);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            lock (_isShuttingDownLock)
            {
                _isShuttingDown = true;
            }

            foreach (Client client in ConnectedClients)
            {
                DisposeClient(client);
            }

           try
            {
                _listenerSocket.Shutdown(SocketShutdown.Receive);

            }
            catch (SocketException e)
            {
                Console.WriteLine("Listening socket was already disconnected.");
            }
            _listenerSocket.Dispose();
            _listenerSocket.Close();
        }

        private void DisposeClient(Client client)
        {
            //client.MessageReceived -= Client_MessageReceived;
            client.Disconnect -= Client_Disconnected;
            client.Dispose();
        }
    }
}
