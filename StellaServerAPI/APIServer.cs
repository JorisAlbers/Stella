using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaServerAPI.Protocol;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerAPI
{
    /// <summary>
    /// Connection between this program and API users.
    /// </summary>
    public class APIServer
    {
        /// <summary> The size of the package protocol TCP buffer </summary>
        private const int BUFFER_SIZE = 1024;

        private readonly string _ip;
        private readonly int _port;
        private readonly List<Storyboard> _storyboards;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private ISocketConnection _listenerSocket;
        private StringProtocol _stringProtocol;
        private BitmapProtocol _bitmapProtocol;

        public List<Client> ConnectedClients { get; set; }

        public EventHandler<Storyboard> StartStoryboard;
        public EventHandler<BitmapReceivedEventArgs> BitmapReceived;


        public APIServer(string ip, int port, List<Storyboard> storyboards)
        {
            _ip = ip;
            _port = port;
            _storyboards = storyboards;
            _stringProtocol = new StringProtocol();
            _bitmapProtocol = new BitmapProtocol();
        }



        public void Start()
        {
            ConnectedClients = new List<Client>();
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_ip), _port);

            Console.Out.WriteLine($"Starting APIServer on {_port}");
            // Create a TCP/IP socket.  
            _listenerSocket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TODO inject with ISocketConnection
            // Bind the socket to the local endpoint and listen for incoming connections.  

            _listenerSocket.Bind(localEndPoint);
            _listenerSocket.Listen(100);

            // Start an asynchronous socket to listen for connections.  
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Console.Out.WriteLine("Received accept callback");
            // Get the socket that handles the client request.  
            ISocketConnection listener = (ISocketConnection)ar.AsyncState;

            lock (_isShuttingDownLock)
            {
                if (_isShuttingDown)
                {
                    Console.WriteLine("Ignored new client callback as the server is shutting down.");
                    return;
                }
            }

            // Start an asynchronous socket to listen for connections.  
            _listenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), _listenerSocket);

            // Handle the new connection
            ISocketConnection handler = listener.EndAccept(ar);

            // Create a new client.
            Client client = new Client(new SocketConnectionController<MessageType>(handler, BUFFER_SIZE));
            client.MessageReceived += Client_MessageReceived;
            client.Disconnect += Client_Disconnected;

            ConnectedClients.Add(client);
            client.Start();
        }

        private void Client_Disconnected(object sender, SocketException e)
        {
            Client client = (Client)sender;
            Console.WriteLine($"Client {client.ID} disconnected, {e.SocketErrorCode}"); // TODO add intended disconnect, not just on SocketException
            ConnectedClients.Remove(client);
            DisposeClient(client);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            Console.Out.WriteLine($"[IN] {e.MessageType}");
            switch (e.MessageType)
            {
                case MessageType.None:
                    break;
                case MessageType.GetAvailableStoryboards:
                    byte[][] data = StringProtocol.Serialize(String.Join(';',_storyboards.Select(x=>x.Name)), 1024);
                    foreach (var package in data)
                    {
                        ((Client)sender).Send(MessageType.GetAvailableStoryboards, package);
                    }
                    break;
                case MessageType.StartPreloadedStoryboard:
                    ParseStartPreloadedStoryboardMessage(e.Message);
                    break;
                case MessageType.StartStoryboard:
                    ParseStartStoryboardMessage(e.Message);
                    break;
               case MessageType.StoreBitmap:
                    ParseStoreBitmapMessage(e.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown message type {e}");
            }
        }

        private void ParseStartStoryboardMessage(byte[] data)
        {
            if (_stringProtocol.TryDeserialize(data, out string storyboardAsYaml))
            {
                // Reset StringProtocol
                _stringProtocol = new StringProtocol();

                StoryboardLoader storyboardLoader = new StoryboardLoader();
                Storyboard storyboard = null;
                try
                {
                    storyboard =
                        storyboardLoader.Load(
                            new StreamReader(
                                new MemoryStream(
                                    Encoding.ASCII.GetBytes(
                                        storyboardAsYaml)))); // TODO te stringProtocol should should return the byte[] . Now we convert from bytes -> string -> bytes.
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("APIserver: Failed to start storyboard.");
                    Console.Out.WriteLine(e);
                    return;
                }

                // Bubble up
                EventHandler<Storyboard> handler = StartStoryboard;
                if (handler != null)
                {
                    handler.Invoke(this, storyboard);
                }
            }
        }

        private void ParseStartPreloadedStoryboardMessage(byte[] data)
        {
            if (_stringProtocol.TryDeserialize(data, out string storyboardName))
            {
                // Reset StringProtocol
                _stringProtocol = new StringProtocol();

                // Get the storyboard
                Storyboard storyboard = _storyboards.FirstOrDefault(x => x.Name == storyboardName);
                if (storyboard == null)
                {
                    Console.Out.WriteLine($"API: failed to start storyboard. Storyboard {storyboardName} does not exist");
                    return;
                }

                // Bubble up
                EventHandler <Storyboard> handler = StartStoryboard;
                if (handler != null)
                {
                    handler.Invoke(this,storyboard);
                }
            }
        }

        private void ParseStoreBitmapMessage(byte[] data)
        {
            if (_bitmapProtocol.TryDeserialize(data, out Bitmap bitmap, out string name))
            {
                // Reset the bitmap protocol
                _bitmapProtocol = new BitmapProtocol();

                // Bubble up
                EventHandler<BitmapReceivedEventArgs> handler = BitmapReceived;
                if (handler != null)
                {
                    handler.Invoke(this, new BitmapReceivedEventArgs{Bitmap = bitmap, Name = name});
                }

            }
        }


        public void Dispose()
        {
            lock (_isShuttingDownLock)
            {
                _isShuttingDown = true;
            }

            foreach (Client client in ConnectedClients)
            {
                DisposeClient(client);
            }

           try
            {
                _listenerSocket.Shutdown(SocketShutdown.Receive);

            }
            catch (SocketException e)
            {
                Console.WriteLine("Listening socket was already disconnected.");
            }
            _listenerSocket.Dispose();
            _listenerSocket.Close();
        }

        private void DisposeClient(Client client)
        {
            client.MessageReceived -= Client_MessageReceived;
            client.Disconnect -= Client_Disconnected;
            client.Dispose();
        }
    }
}
