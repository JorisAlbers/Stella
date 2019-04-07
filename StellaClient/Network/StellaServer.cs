using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        private string _id;
        private ISystemTimeSetter _systemTimeSetter;
        private TimeSetter _timeSetter;
        private SocketConnectionController _socketConnectionController;
        private object _resourceLock = new object();

        private Dictionary<int, FrameProtocol> _frameSectionBuffer; // int = frame index, 

        public event EventHandler<FrameSetMetadata> AnimationStartReceived;
        public event EventHandler<Frame> FrameReceived;


        public StellaServer(IPEndPoint serverAdress, string ID, ISystemTimeSetter timeSetter)
        {
            _serverAdress = serverAdress;
            _id = ID;
            _systemTimeSetter = timeSetter;
        }

        public void Start()
        {
            Connect();
        }


        public void Send(MessageType type, byte[] message)
        {
            _socketConnectionController.Send(type,message);
        }

        public void SendFrameRequest(int? lastFrameIndex, int count)
        {
            Send(MessageType.Animation_Request, AnimationRequestProtocol.CreateRequest(lastFrameIndex?? 0, count));
        }

        private void SendInit()
        {
            Send(MessageType.Init,Encoding.ASCII.GetBytes(_id));
        }
        
        private void Connect()
        {
            Console.Out.WriteLine($"Trying to connect with server on ip {_serverAdress.Address}:{_serverAdress.Port}");
            // Create a TCP/IP socket.  
            ISocketConnection socket = new SocketConnection(_serverAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject
  
            // Connect to the remote endpoint.  
            socket.BeginConnect(_serverAdress, new AsyncCallback(ConnectCallback), socket);  
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try 
            {  
                // Complete the connection.  
                ISocketConnection socket = (ISocketConnection) ar.AsyncState;
                socket.EndConnect(ar);

                Console.WriteLine("Connected with StellaServer");
                _socketConnectionController = new SocketConnectionController(socket);
                _socketConnectionController.MessageReceived += OnMessageReceived;
                _socketConnectionController.Start();

                // Make sure the time of the server is synced with our time
                if(!_systemTimeSetter.TimeIsNTPSynced()) // TODO remember if time is synced in case the StellaServer object crashes
                {
                    _timeSetter = new TimeSetter(_systemTimeSetter,9);
                    Send(MessageType.TimeSync, TimeSyncProtocol.CreateMessage(DateTime.Now));
                }
            } 
            catch (SocketException e) 
            {  
                Console.WriteLine($"Failed to open connection with StellaServer\n {e.ToString()}");
                //TODO OnDisconnect
            }  
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine($"[IN]  [{e.MessageType}] {e.Message}");
            switch(e.MessageType)
            {
                case MessageType.Init: // Server wants us to send our init values
                    SendInit();
                    break;
                case MessageType.TimeSync: // Server sends back a timesync message
                    ParseTimeSyncData(e.Message);
                    break;
                case MessageType.Animation_Start: // Server wants us to start a new animation
                    OnAnimationStartReceived(e.Message);
                    break;
                case MessageType.Animation_Request: // Server sends us frames
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

        private void OnAnimationStartReceived(byte[] message)
        {
            FrameSetMetadata metadata = FrameSetMetadataProtocol.Deserialize(message);
            Console.Out.WriteLine($"Animation start request received.");
            lock (_resourceLock)
            {
                _frameSectionBuffer = new Dictionary<int, FrameProtocol>();
            }
            EventHandler<FrameSetMetadata> handler = AnimationStartReceived;
            if (handler != null)
            {
                handler(this, metadata);
            }
        }

        private void OnAnimationRequestReceived(byte[] message)
        {
            // The frame might be split up into multiple packages. 
            // We keep a buffer (FrameProtocol for each frame) to wait for all packages.
            // When the frame is complete, we call FrameReceived.
            int frameIndex = FrameProtocol.GetFrameIndex(message);
            Frame frame = null;
            lock (_resourceLock)
            {
                if (!_frameSectionBuffer.ContainsKey(frameIndex))
                {
                    _frameSectionBuffer.Add(frameIndex, new FrameProtocol());
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

        private void OnFrameReceived(Frame frame)
        {
            EventHandler<Frame> handler = FrameReceived;
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