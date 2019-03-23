using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class SocketConnection
    {
        private PacketProtocol _packetProtocol;
        private bool _isDisposed = false;
        private Socket _socket;
        private readonly object _parsingMessageLock = new object(); // Lock used by each message parsing thread
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Disconnect;
        
        public bool IsConnected {get; private set;}

        public SocketConnection(Socket socket)
        {
            _socket = socket;
        }

        public void Start()
        {
            if(!_socket.Connected)
            {
                throw new Exception("The socket must be connected before starting the SocketConnection");
            }

            _packetProtocol = new PacketProtocol();
            _packetProtocol.MessageArrived = (MessageType, data)=> OnMessageReceived(MessageType,data);

            byte[] buffer = new byte[PacketProtocol.BUFFER_SIZE];
            IsConnected = true;
            _socket.BeginReceive(buffer, 0, PacketProtocol.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);  
        }


        // -------------------- SEND -------------------- \\
        public void Send(MessageType messageType, byte[] message)
        {
            if(!IsConnected)
            {
                throw new Exception("SocketConnection is not connected"); 
            }
            if(_isDisposed)
            {
                throw new ObjectDisposedException("SocketConnection has been disposed");
            }

            byte[] data = PacketProtocol.WrapMessage(messageType, message);  

            Console.WriteLine($"[OUT] [{messageType}] length:{message.Length}");

            // Begin sending the data to the remote device.  
            try
            {
                _socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), _socket);  
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to send data");
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
                Console.WriteLine("Failed to send data, "+e.ToString());
            }
            catch (SocketException e) 
            {  
                Console.WriteLine("Failed to send data, connection lost "+e.ToString());
                OnDisconnect(new EventArgs());
            }  
        }

        // -------------------- Receive -------------------- \\

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Read incoming data from the client.
            if(_isDisposed)
            {
                Console.WriteLine($"Ignored incoming message as this object is disposed.");
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
                // First start receiving more data
                byte[] buffer = new byte[PacketProtocol.BUFFER_SIZE];
                _socket.BeginReceive(buffer, 0, PacketProtocol.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);  

                // Then parse the message
                lock(_parsingMessageLock) // TODO FIFO is not guaranteed, maybe add QueuedLock
                {
                    try
                    {
                        byte[] b = (byte[]) ar.AsyncState;
                        _packetProtocol.DataReceived(b.Take(bytesRead).ToArray());
                    }
                    catch(ProtocolViolationException e)
                    {
                        Console.WriteLine("Failed to receive data. Package protocol violation. \n"+e.ToString());
                        _packetProtocol.MessageArrived = null;
                        _packetProtocol.KeepAliveArrived = null;
                        _packetProtocol = new PacketProtocol();
                        _packetProtocol.MessageArrived = (MessageType, data)=> OnMessageReceived(MessageType,data);
                    }
                }
            }  
        }

        protected virtual void OnMessageReceived(MessageType type, byte[] bytes)
        {
            Console.WriteLine($"[IN]  [{_socket.RemoteEndPoint as IPEndPoint}] [{type}] bytes received : {bytes.Length}");

            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageReceivedEventArgs()
                {
                    MessageType = type,
                    Message = bytes
                });
            }
        }

        protected virtual void OnDisconnect(EventArgs e)
        {
            // The disconnect event can be called only once.
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