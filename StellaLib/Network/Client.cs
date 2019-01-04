using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StellaLib.Network
{
    public class Client : IDisposable
    {
        private const int BUFFER_SIZE = 1024;
        // Buffer for a single package
        private byte[] _packageBuffer;
        // Buffer for a single message
        private StringBuilder _messageBuffer;
        private bool _isDisposed = false;
        private Socket _socket;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Disconnect;

        public bool IsConnected;

        public Client(Socket socket)
        {
            _socket = socket;
            _packageBuffer = new byte[BUFFER_SIZE];
            _messageBuffer = new StringBuilder();
            IsConnected = true;
            _socket.BeginReceive(_packageBuffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), null);  
        }

        // -------------------- SEND -------------------- \\

        public void Send(MessageType messageType, string data)
        {
            if(!IsConnected)
            {
                throw new ClientDisconnectedException("ID"); // todo add ID
            }
            if(_isDisposed)
            {
                throw new ObjectDisposedException("ID"); // TODO add ID
            }

            string message = $"{messageType};{data}<EOF>";
            Console.WriteLine($"[OUT] {message}");

            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(message);  
    
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
                Console.WriteLine($"Ignored message from client as this object is disposed. Buffer reads {_messageBuffer.ToString()}");
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
                // There  might be more data, so store the data received so far.  
                _messageBuffer.Append(Encoding.ASCII.GetString(_packageBuffer, 0, bytesRead));  
                                
                // Check for end-of-file tag. If it is not there, read more data.  
                string content = _messageBuffer.ToString();  
                if (content.IndexOf("<EOF>") > -1) 
                {  
                    // Reset all buffers
                    _packageBuffer = new byte[BUFFER_SIZE];
                    _messageBuffer = new StringBuilder();

                    _socket.BeginReceive(_packageBuffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), null);  

                    //Pass the content of the message
                    ParseMessage(content);
                } 
                else 
                {  
                    // Not all data received. Get more.  
                    _socket.BeginReceive(_packageBuffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), null);  
                }  
            }  
        }

        private void ParseMessage(string message)
        {
            Console.WriteLine($"[IN]  [{_socket.RemoteEndPoint as IPEndPoint}] {message}");
            // message = <MessageType>;<Message>
            string[] data = message.Split(';');
            if(data.Length != 2)
            {
                Console.WriteLine("Invalid format, the message must be separated by ';' and have a message type and a message.");
                return;
            }

            MessageType messageType;
            if(!Enum.TryParse(data[0], out messageType))
            {
                Console.WriteLine($"Invalid format, Unknown message type : {data[0]}");
                return;
            }

            // remove the <EOF>.
            // TODO replace with length-prefix message
            OnMessageReceived(messageType,data[1].Substring(0,data[1].Length - 5));
        }

        protected virtual void OnMessageReceived(MessageType messageType, string message)
        {
            EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageReceivedEventArgs()
                {
                    MessageType = messageType,
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
            _socket.Disconnect(false);
            _socket.Dispose();
            _socket.Close();
        }
    }

    
}