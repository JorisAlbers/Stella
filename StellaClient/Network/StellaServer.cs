using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using StellaClient.Time;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Packages;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;

namespace StellaClient.Network
{
    /// <summary>
    /// The connection with StellaServer
    /// </summary>
    public class StellaServer :  IStellaServer, IDisposable
    {
        private const int RECONNECT_COOLDOWN_SECONDS = 5;

        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        private readonly int _udpPort;
        private int _id;
        private TimeSetter _timeSetter; // null if the time is NTP synched or when the time has already been synced
        private SocketConnectionController<MessageType> _socketConnectionController;
        private UdpSocketConnectionController<MessageType> _udpSocketConnectionController;
        private object _resourceLock = new object();

        private Dictionary<int, FrameWithoutDeltaProtocol> _frameSectionBuffer; // int = frame index, 

        public event EventHandler RenderFrameReceived;
        public event EventHandler<FrameWithoutDelta> FrameReceived;


        public StellaServer(IPEndPoint serverAdress, int udpPort, int ID, ISystemTimeSetter systemTimeSetter)
        {
            _serverAdress = serverAdress;
            _udpPort = udpPort;
            _id = ID;
            if (!systemTimeSetter.TimeIsNTPSynced())
            {
                // We want to sync the time with the server
                _timeSetter = new TimeSetter(systemTimeSetter, 9);
            }

            _frameSectionBuffer = new Dictionary<int, FrameWithoutDeltaProtocol>();
        }

        public void Start()
        {
            // Start UDP connection
            IPEndPoint udpRemoteEndpoint = new IPEndPoint(_serverAdress.Address, _udpPort);
            IPEndPoint udpLocalEndPoint = new IPEndPoint(IPAddress.Any, _udpPort); // TODO inject the local endpoint?

            ISocketConnection udpSocket = UdpSocketConnectionController<MessageType>.CreateSocket(udpLocalEndPoint);
            _udpSocketConnectionController = new UdpSocketConnectionController<MessageType>(udpSocket, udpRemoteEndpoint);
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
                _socketConnectionController = new SocketConnectionController<MessageType>(socket);
                _socketConnectionController.MessageReceived += OnMessageReceived;
                _socketConnectionController.Disconnect += OnDisconnect;
                _socketConnectionController.Start();
                
                // Send init
                SendInit();

                // Make sure the time of the server is synced with our time
                if(_timeSetter != null)
                {
                    Send(MessageType.TimeSync, TimeSyncProtocol.CreateMessage(DateTime.Now));
                }
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
                case MessageType.TimeSync: // Server sends back a timesync message
                    ParseTimeSyncData(e.Message);
                    break;
                case MessageType.Animation_RenderFrame: // Server wants us to render the prepared frame
                    OnAnimationStartReceived();
                    break;
                case MessageType.Animation_PrepareFrame: // Server sends us frames
                    OnAnimationRequestReceived(e.Message);
                    break;
                default:
                    Console.WriteLine($"MessageType {e.MessageType} is not used by StellaClient.");
                    break;
            }
        }

        private void ParseTimeSyncData(byte[] data)
        {
           if(_timeSetter == null)
           {
               Console.WriteLine("TimeSync message from server ignored as the time setter is null");
               return;
           }

           long[] measurements;
           try
           {
               measurements = TimeSyncProtocol.ParseMessage(data);
           }
           catch(Exception e)
           {
               Console.WriteLine($"Failed to parse TimeSync message from server.");
               return;
           }
           
           _timeSetter.AddMeasurements(measurements[0],measurements[1],DateTime.Now.Ticks);
           if(_timeSetter.NeedsMoreData)
           {
               // Start the next timeSync measurement
               Send(MessageType.TimeSync, TimeSyncProtocol.CreateMessage(DateTime.Now));
           }
           else
           {
               Console.WriteLine("TimeSync completed.");
               _timeSetter = null;
           }
        }

        private void OnAnimationStartReceived()
        {
            EventHandler handler = RenderFrameReceived;
            if (handler != null)
            {
                handler(this,new EventArgs());
            }
        }

        private void OnAnimationRequestReceived(byte[] message)
        {
            // The frame might be split up into multiple packages. 
            // We keep a buffer (FrameProtocol for each frame) to wait for all packages.
            // When the frame is complete, we call FrameReceived.
            int frameIndex = FrameWithoutDeltaProtocol.GetFrameIndex(message);
            FrameWithoutDelta frame = null;
            lock (_resourceLock)
            {
                if (_frameSectionBuffer == null)
                {
                    _frameSectionBuffer = new Dictionary<int, FrameWithoutDeltaProtocol>();
                }

                if (!_frameSectionBuffer.ContainsKey(frameIndex))
                {
                    _frameSectionBuffer.Add(frameIndex, new FrameWithoutDeltaProtocol());
                }

                if (_frameSectionBuffer[frameIndex].TryDeserialize(message, out frame))
                {
                   _frameSectionBuffer.Remove(frameIndex);
                }
            }

            if (frame != null)
            {
                OnFrameReceived(frame);
            }
        }

        private void OnFrameReceived(FrameWithoutDelta frame)
        {
            EventHandler<FrameWithoutDelta> handler = FrameReceived;
            if (handler != null)
            {
                handler(this, frame);
            }
        }


        public void Dispose()
        {
            _isDisposed = true;
            _socketConnectionController.Dispose();
        }
    }
}