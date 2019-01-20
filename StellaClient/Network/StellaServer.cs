using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;

namespace StellaClient.Network
{
    /// <summary>
    /// The connection with StellaServer
    /// </summary>
    public class StellaServer : IDisposable
    {
        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        private string _id;
        private SocketConnection _socketConnection;
        
        public StellaServer(IPEndPoint serverAdress, string ID)
        {
            _serverAdress = serverAdress;
            _id = ID;
        }

        public void Start()
        {
            Connect();
        }

        public void Send(MessageType type, string message)
        {
            _socketConnection.Send(type,message);
        }

        private void SendInit()
        {
            Send(MessageType.Init,_id);
        }
        
        private void Connect()
        {
            // Create a TCP/IP socket.  
            Socket socket = new Socket(_serverAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
  
            // Connect to the remote endpoint.  
            socket.BeginConnect(_serverAdress, new AsyncCallback(ConnectCallback), socket);  
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try 
            {  
                // Complete the connection.  
                Socket socket = (Socket) ar.AsyncState;
                socket.EndConnect(ar);

                Console.WriteLine("Connected with StellaServer");
                _socketConnection = new SocketConnection(socket);
                _socketConnection.MessageReceived += OnMessageReceived;
                _socketConnection.Start();
            } 
            catch (SocketException e) 
            {  
                Console.WriteLine($"Failed to open connection with StellaServer\n {e.ToString()}");
                //TODO OnDisconnect
            }  
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"[IN] [{e.MessageType}] {e.Message}");
            switch(e.MessageType)
            {
                case MessageType.Init: // Server wants us to send our init values
                    SendInit();
                    break;
                default:
                    Console.WriteLine($"MessageType {e.MessageType} is not used by StellaClient.");
                    break;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _socketConnection.Dispose();
        }
    }
}