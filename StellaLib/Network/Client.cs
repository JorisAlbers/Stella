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

        public Client(Socket socket)
        {
            _socket = socket;
            _packageBuffer = new byte[BUFFER_SIZE];
            _messageBuffer = new StringBuilder();

            _socket.BeginReceive(_packageBuffer, 0, BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), null);  
        }

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

        public void Dispose()
        {
            _isDisposed = true;
            _socket.Disconnect(false);
            _socket.Dispose();
            _socket.Close();
        }
    }
}