using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol.Animation;
using StellaServerLib.Network;

namespace StellaClientLib.Network
{
    /// <summary>
    /// The connection with StellaServer
    /// </summary>
    public class StellaServer :  IStellaServer, IDisposable
    {
        private const int RECONNECT_COOLDOWN_SECONDS = 5;
        /// <summary> The size of the package protocol TCP buffer </summary>
        private const int TCP_BUFFER_SIZE = 1024;
        /// <summary> The size of the package protocol UDP buffer </summary>
        private const int UDP_BUFFER_SIZE = 60_000; // The maximum UDP package size is 65,507 bytes.

        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        private int _id;
        private SocketConnectionController<MessageType> _socketConnectionController;
        private UdpSocketConnectionController<MessageType> _udpSocketConnectionController;
        private object _resourceLock = new object();

        private Dictionary<int, FrameProtocol> _frameSectionBuffer; // int = frame index, 

        public event EventHandler<FrameWithoutDelta> RenderFrameReceived;


        public StellaServer()
        {
            _frameSectionBuffer = new Dictionary<int, FrameProtocol>();
        }

        public void Start(IPEndPoint serverAdress, int udpPort, int Id)
        {
            _serverAdress = serverAdress;
            _id = Id;
            // Start UDP connection
            IPEndPoint udpRemoteEndpoint = new IPEndPoint(_serverAdress.Address, udpPort);
            IPEndPoint udpLocalEndPoint = new IPEndPoint(IPAddress.Any, udpPort); // TODO inject the local endpoint?

            SocketConnectionCreator socketConnectionCreator = new SocketConnectionCreator();
            ISocketConnection udpSocket = socketConnectionCreator.Create(udpLocalEndPoint);
            _udpSocketConnectionController = new UdpSocketConnectionController<MessageType>(udpSocket, udpRemoteEndpoint, UDP_BUFFER_SIZE);
            _udpSocketConnectionController.MessageReceived += OnMessageReceived;
            _udpSocketConnectionController.Start();

            Connect();
        }


        public void Send(MessageType type, byte[] message)
        {
            try
            {
                if (_socketConnectionController.IsConnected)
                {
                    _socketConnectionController.Send(type, message);
                }
                else
                {
                    Console.Out.WriteLine($"Failed to send message of type {type}. Not connected with server."); // TODO keep buffer of message to send on reconnect
                }
            }
            catch (SocketException exception)
            {
                Console.Out.WriteLine($"Failed to send message of type {type}. {exception}"); // TODO keep buffer of message to send on reconnect
            }
        }

        private void SendInit()
        {
            Send(MessageType.Init, BitConverter.GetBytes(_id));
        }
        
        private void Connect()
        {
            Console.Out.WriteLine($"Trying to connect with server on ip {_serverAdress.Address}:{_serverAdress.Port}");
            // Create a TCP/IP socket.  
            ISocketConnection socket = new SocketConnection(_serverAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject

            // Connect to the remote endpoint.
            try
            {
                socket.BeginConnect(_serverAdress, new AsyncCallback(ConnectCallback), socket);
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Failed to open connection with StellaServer.{e.SocketErrorCode}.");
                Reconnect();
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try 
            {  
                // Complete the connection.  
                ISocketConnection socket = (ISocketConnection) ar.AsyncState;
                socket.EndConnect(ar);

                Console.WriteLine("Connected with StellaServer");
                _socketConnectionController = new SocketConnectionController<MessageType>(socket, TCP_BUFFER_SIZE);
                _socketConnectionController.MessageReceived += OnMessageReceived;
                _socketConnectionController.Disconnect += OnDisconnect;
                _socketConnectionController.Start();
                
                // Send init
                SendInit();
            } 
            catch (SocketException e) 
            {  
                Console.WriteLine($"Failed to open connection with StellaServer.{e.SocketErrorCode}.");
                Reconnect();
            }
        }

        private void OnDisconnect(object sender, SocketException e)
        {
            Console.Out.WriteLine($"Lost connection with the server, {e.SocketErrorCode}.");
            _socketConnectionController.Disconnect -= OnDisconnect;
            _socketConnectionController.MessageReceived -= OnMessageReceived;
            _socketConnectionController.Dispose();

            Reconnect();
        }

        private async void Reconnect()
        {
            await Task.Delay(RECONNECT_COOLDOWN_SECONDS * 1000);
            Connect();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            switch(e.MessageType)
            {
                case MessageType.Init: // Server wants us to send our init values
                    SendInit();
                    break;
                case MessageType.AnimationRenderFrame: // Server wants us to render a frame
                    OnAnimationRequestReceived(e.Message);
                    break;
                default:
                    Console.WriteLine($"MessageType {e.MessageType} is not used by StellaClient.");
                    break;
            }
        }

        private void OnAnimationRequestReceived(byte[] message)
        {
            // The frame might be split up into multiple packages. 
            // We keep a buffer (FrameProtocol for each frame) to wait for all packages.
            // When the frame is complete, we call FrameReceived.
            return; // TODO remove this entire lib
        }

        private void OnFrameReceived(FrameWithoutDelta frame)
        {
            EventHandler<FrameWithoutDelta> handler = RenderFrameReceived;
            if (handler != null)
            {
                handler(this, frame);
            }
        }


        public void Dispose()
        {
            _isDisposed = true;
            _socketConnectionController.Dispose();
            _udpSocketConnectionController.Dispose();
        }
    }
}