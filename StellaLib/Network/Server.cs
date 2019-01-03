using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace StellaLib.Network
{
    public class Server
    {
        private List<Client> _newConnections;

        private int _port;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private Socket _listenerSocket;

        public ManualResetEvent allDone = new ManualResetEvent(false);  

        public Server(int port)
        {
            _port = port;
            _newConnections =  new List<Client>();
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

        private void AcceptCallback(IAsyncResult ar) 
        {  
            // Signal the main thread to continue.  
            allDone.Set();  
    
            // Get the socket that handles the client request.  
            Socket listener = (Socket) ar.AsyncState;  

            lock(_isShuttingDownLock)
            {
                if(_isShuttingDown)
                {
                    Console.WriteLine("Ignored new client callback as the server is shutting down.");
                    return;
                }

                // Start an asynchronous socket to listen for connections.  
                _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);

                // Handle the new connection
                Socket handler = listener.EndAccept(ar); 
                // Create a new client.
                Client client = new Client(handler);
                // As we do not now wich client this is, add him to the list of new connection.
                _newConnections.Add(client);
            }
        }  
    }
}