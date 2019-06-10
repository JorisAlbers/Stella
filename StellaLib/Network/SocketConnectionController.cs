using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Timers;
using StellaLib.Network.Protocol;

namespace StellaLib.Network
{
    public class SocketConnectionController
    {
        private PacketProtocol _packetProtocol;
        private bool _isDisposed = false;
        private ISocketConnection _socket;
        private readonly object _parsingMessageLock = new object(); // Lock used by each message parsing thread
        private System.Timers.Timer _keepAliveTimer;
        private const int KEEP_ALIVE_TIMER_INTERVAL = 2000; // Send a keep alive message every x seconds

        public event EventHandler<MessageReceivedEventArgs<MessageType>> MessageReceived;
        public event EventHandler<SocketException> Disconnect;
        
        public bool IsConnected {get; private set;}

        public SocketConnectionController(ISocketConnection socket)
        {
            _socket = socket;
        }

        public void Start()
        {
            if(!_socket.Connected)
            {
                throw new Exception("The socket must be connected before starting the SocketConnectionController");
            }

            _packetProtocol = new PacketProtocol();
            _packetProtocol.MessageArrived = (MessageType, data)=> OnMessageReceived(MessageType,data);
            IsConnected = true;
            byte[] buffer = new byte[PacketProtocol.BUFFER_SIZE];
            _socket.BeginReceive(buffer, 0, PacketProtocol.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);

            // Start sending KeepAlive packages to check if the connection is still open
            _keepAliveTimer = new System.Timers.Timer();
            _keepAliveTimer.Interval = KEEP_ALIVE_TIMER_INTERVAL;
            _keepAliveTimer.Elapsed += new ElapsedEventHandler(KeepAliveCallback);
            _keepAliveTimer.Enabled = true;
        }

        private void KeepAliveCallback(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!IsConnected || _isDisposed)
            {
                return;
            }

            try
            {
                byte[] keepAliveBytes = PacketProtocol.WrapKeepaliveMessage();

                _socket.BeginSend(keepAliveBytes, 0, keepAliveBytes.Length, 0, new AsyncCallback(SendCallback), _socket);
            }
            catch (SocketException e)
            {
                OnDisconnect(e);
            }
        }


        // -------------------- SEND -------------------- \\
        public void Send(MessageType messageType, byte[] message)
        {
            if(!IsConnected)
            {
                throw new Exception("SocketConnectionController is not connected"); 
            }
            if(_isDisposed)
            {
                throw new ObjectDisposedException("SocketConnectionController has been disposed");
            }

            byte[] data = PacketProtocol.WrapMessage(messageType, message);

            // Log the incoming message. Ignoring often occurring types.
            if (messageType != MessageType.Animation_Request)
            {
                Console.WriteLine($"[OUT] [{messageType}] length:{message.Length}");
            }

            // Begin sending the data to the remote device.  
            try
            {
                _socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), _socket);  
            }
            catch (SocketException e)
            {
                OnDisconnect(e);
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
                Console.WriteLine("Failed to send data as socket was disposed.");
            }
            catch (SocketException e) 
            {  
                OnDisconnect(e);
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
                OnDisconnect(e);
                return;
            }

            if (bytesRead > 0)
            {
                // Parse the message
                lock(_parsingMessageLock)
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
                // Start receiving more data
                byte[] buffer = new byte[PacketProtocol.BUFFER_SIZE];
                _socket.BeginReceive(buffer, 0, PacketProtocol.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), buffer);
            }  
        }

        protected virtual void OnMessageReceived(MessageType type, byte[] bytes)
        {
            // Log the incoming message. Ignoring often occurring types.
            if (type != MessageType.Animation_Request)
            {
                Console.WriteLine($"[IN]  [{_socket.RemoteEndPoint as IPEndPoint}] [{type}] bytes received : {bytes.Length}");
            }

            EventHandler<MessageReceivedEventArgs<MessageType>> handler = MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageReceivedEventArgs<MessageType>()
                {
                    MessageType = type,
                    Message = bytes
                });
            }
        }

        protected virtual void OnDisconnect(SocketException exception)
        {
            // The disconnect event can be called only once.
            if(IsConnected)
            {
                IsConnected = false;
                EventHandler<SocketException> handler = Disconnect;
                if (handler != null)
                {
                    handler(this, exception);
                }
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            _keepAliveTimer.Enabled = false;
            _keepAliveTimer.Stop();
            _packetProtocol.MessageArrived = null;
            _packetProtocol.KeepAliveArrived = null;
            _socket.Disconnect(false);
            _socket.Dispose();
            _socket.Close();
        }
    }
}