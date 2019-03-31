using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaClient.Time;
using StellaLib.Animation;
using StellaLib.Network;
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
        private SocketConnection _socketConnection;

        public event EventHandler<FrameSetMetadata> AnimationStartReceived;


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
            _socketConnection.Send(type,message);
        }

        private void SendInit()
        {
            Send(MessageType.Init,Encoding.ASCII.GetBytes(_id));
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

                // Make sure the time of the server is synced with our time
                if(!_systemTimeSetter.TimeIsNTPSynced()) // TODO remember if time is synced in case the StellaServer object crashes
                {
                    _timeSetter = new TimeSetter(_systemTimeSetter,9);
                    Send(MessageType.TimeSync, TimeSyncProtocol.CreateMessage());
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
               Send(MessageType.TimeSync, TimeSyncProtocol.CreateMessage());
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
            EventHandler<FrameSetMetadata> handler = AnimationStartReceived;
            if (handler != null)
            {
                handler(this, metadata);
            }
        }


        public void Dispose()
        {
            _isDisposed = true;
            _socketConnection.Dispose();
        }
    }
}