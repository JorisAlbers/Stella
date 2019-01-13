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
        private const int BUFFER_SIZE = 1024;
        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        Socket _socket;
        PacketProtocol _packetProtocol;

        public bool IsConnected {get;private set;}

        public StellaServer(IPEndPoint serverAdress)
        {
            _serverAdress = serverAdress;
            _packetProtocol = new PacketProtocol(BUFFER_SIZE);
        }

        public void Start()
        {
            // Create a TCP/IP socket.  
            _socket = new Socket(_serverAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            _socket.BeginConnect(_serverAdress, new AsyncCallback(ConnectCallback), null);  
        }

        public void Send(MessageType type, string message)
        {
            if(!IsConnected)
            {
                throw new Exception("Failed to send message, StellaServer not connected"); // todo add ID
            }
            if(_isDisposed)
            {
                throw new ObjectDisposedException("StellaServer has been disposed."); // TODO add ID
            }
            
            Console.WriteLine($"[OUT] [{type}] {message}");
            // Convert the messageType and message to the PackageProtocol
            byte[] byteData = PacketProtocol.WrapMessage(type, message);  
    
            // Begin sending the data to the remote device.  
            try
            {
                _socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), _socket);  
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to send data to server.");
                Console.WriteLine(e.ToString());  
                //TODO OnDisconnect(new EventArgs());
            }
        }

        private void SendCallback(IAsyncResult ar) 
        {  
            try 
            {  
                // Complete sending the data to the remote device.  
                int bytesSent = _socket.EndSend(ar);
            }
            catch(ObjectDisposedException e)
            {
                Console.WriteLine("Failed to send data to the server, "+e.ToString());
            }
            catch (SocketException e) 
            {  
                Console.WriteLine("Failed to send data to the server, connection lost "+e.ToString());
                //TODO OnDisconnect(new EventArgs());
            }  
        }
        
        private void ConnectCallback(IAsyncResult ar)
        {
            try 
            {  
                // Complete the connection.  
                _socket.EndConnect(ar);
                IsConnected = true;
                Console.WriteLine("Connected with StellaServer");

                byte[] buffer = new byte[1024];
                _socket.BeginReceive(buffer,0,BUFFER_SIZE,0,new AsyncCallback(ReceiveCallback),buffer);    
            } 
            catch (SocketException e) 
            {  
                Console.WriteLine($"Failed to open connection with StellaServer\n {e.ToString()}");
                //TODO OnDisconnect
            }  
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Read incoming data from the client.
            if(_isDisposed)
            {
                Console.WriteLine($"Ignored message from server as this object is disposed.");
                return;
            }

            int bytesRead = 0;
            try
            {
                bytesRead = _socket.EndReceive(ar);
            }
            catch(SocketException e)
            {
                Console.WriteLine("Receive data failed.\n"+e.ToString());
                //TODO OnDisconnect
                return;
            }

            if (bytesRead > 0) 
            {  
                // First pass the buffer to the packetProtocol to parse the message
                try
                {
                    byte[] b = (byte[]) ar.AsyncState;
                    _packetProtocol.DataReceived(b.Take(bytesRead).ToArray());
                }
                catch(ProtocolViolationException e)
                {
                    Console.WriteLine("Failed to receive data from server. Package protocol violation. \n"+e.ToString());
                    _packetProtocol.MessageArrived = null;
                    _packetProtocol.KeepAliveArrived = null;
                    _packetProtocol = new PacketProtocol(BUFFER_SIZE);
                    _packetProtocol.MessageArrived = (MessageType, data)=> OnMessageReceived(MessageType,data);
                }
                
                // Then listen for more data
                byte[] buffer = new byte[BUFFER_SIZE];
                _socket.BeginReceive(buffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);  
            }  
        }

        private void OnMessageReceived(MessageType type, byte[] data)
        {
            string message = Encoding.ASCII.GetString(data);
            Console.WriteLine($"[IN] [{type}] {message}");
            switch(type)
            {
                case MessageType.Init:
                    // Server wants us to send init value
                    break;
                default:
                    Console.WriteLine($"MessageType {type} is not used by StellaClient.");
                    break;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _packetProtocol.MessageArrived = null;
            _packetProtocol.KeepAliveArrived = null;
            _socket.Disconnect(false);
            _socket.Dispose();
            _socket.Close();
        }




    }
}