using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;

namespace StellaServer.Network
{
    public class Client : IDisposable
    {
        private const int BUFFER_SIZE = 1024;

        private PacketProtocol _packetProtocol;
        private bool _isDisposed = false;
        private Socket _socket;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Disconnect;

        public bool IsConnected {get; private set;}

        public string ID = null;

        public Client(Socket socket)
        {
            _socket = socket;
            _packetProtocol = new PacketProtocol(BUFFER_SIZE);
            _packetProtocol.MessageArrived = (MessageType, data)=> OnMessageReceived(MessageType,data);
            //_packageBuffer = new byte[BUFFER_SIZE];
            byte[] buffer = new byte[BUFFER_SIZE];
            IsConnected = true;
            _socket.BeginReceive(buffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);  
        }

        // -------------------- SEND -------------------- \\

        public void Send(MessageType messageType, string message)
        {
            if(!IsConnected)
            {
                throw new ClientDisconnectedException("ID"); // todo add ID
            }
            if(_isDisposed)
            {
                throw new ObjectDisposedException("ID"); // TODO add ID
            }

            Console.WriteLine($"[OUT] {message}");

            // Convert the messageType and message to the PackageProtocol
            byte[] byteData = PacketProtocol.WrapMessage(messageType, message);  
    
            // Begin sending the data to the remote device.  
            try
            {
                _socket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), _socket);  
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to send data to client.");
                Console.WriteLine(e.ToString());  
                OnDisconnect(new EventArgs());
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
                Console.WriteLine("Failed to send data to the client, "+e.ToString());
            }
            catch (SocketException e) 
            {  
                Console.WriteLine("Failed to send data to the client, connection lost "+e.ToString());
                OnDisconnect(new EventArgs());
            }  
        }

        // -------------------- Receive -------------------- \\

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Read incoming data from the client.
            if(_isDisposed)
            {
                Console.WriteLine($"Ignored message from client as this object is disposed.");
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
                OnDisconnect(new EventArgs());
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
                    Console.WriteLine("Failed to receive data from client. Package protocol violation. \n"+e.ToString());
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

        protected virtual void OnMessageReceived(MessageType type, byte[] bytes)
        {
            string message = Encoding.ASCII.GetString(bytes);
            Console.WriteLine($"[IN]  [{_socket.RemoteEndPoint as IPEndPoint}] [{type}] {message}");

            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageReceivedEventArgs()
                {
                    MessageType = type,
                    Message = message
                });
            }
        }

        protected virtual void OnDisconnect(EventArgs e)
        {
            // The disconnect event can be called only once. A new client will be initialized on reconnect.
            if(IsConnected)
            {
                IsConnected = false;
                EventHandler handler = Disconnect;
                if (handler != null)
                {
                    handler(this, e);
                }
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