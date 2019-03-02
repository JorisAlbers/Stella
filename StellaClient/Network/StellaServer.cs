using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaClient.Time;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaClient.Network
{
    /// <summary>
    /// The connection with StellaServer
    /// </summary>
    public class StellaServer : IDisposable
    {
        private bool _isDisposed;
        private IPEndPoint _serverAdress;
        private string _id;
        private ISystemTimeSetter _systemTimeSetter;
        private TimeSetter _timeSetter;
        private SocketConnection _socketConnection;
        
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

        public void Send(MessageType type, string message)
        {
            _socketConnection.Send(type,message);
        }

        private void SendInit()
        {
            Send(MessageType.Init,_id);
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
                    _timeSetter = new TimeSetter(_systemTimeSetter);
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
                    ParseTimeSyncMessage(e.Message);
                    break;
                default:
                    Console.WriteLine($"MessageType {e.MessageType} is not used by StellaClient.");
                    break;
            }
        }

        private void ParseTimeSyncMessage(string message)
        {
           if(_timeSetter == null)
           {
               Console.WriteLine("TimeSync message from server ignored as the time setter is null");
               return;
           }

           long[] measurements;
           try
           {
               measurements = TimeSyncProtocol.ParseMessage(message);
           }
           catch(Exception e)
           {
               Console.WriteLine($"Failed to parse TimeSync message from server. Message = {message}");
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

        public void Dispose()
        {
            _isDisposed = true;
            _socketConnection.Dispose();
        }
    }
}