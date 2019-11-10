﻿using System;
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
        private readonly List<IAnimation> _animations;
        private bool _isShuttingDown = false;
        private object _isShuttingDownLock = new object();

        private ISocketConnection _listenerSocket;
        private StringProtocol _stringProtocol;
        private BitmapProtocol _bitmapProtocol;

        public List<Client> ConnectedClients { get; set; }

        public EventHandler<IAnimation> StartAnimation;
        public EventHandler<BitmapReceivedEventArgs> BitmapReceived;
        
        // Transform events
        public event Func<int, int> TimeUnitsPerFrameRequested; 
        public event Action<int,int> TimeUnitsPerFrameSet;

        public event Func<int, float[]> RgbFadeRequested;
        public event Action<int, float[]> RgbFadeSet;

        public event Func<int, float> BrightnessCorrectionRequested;
        public event Action<int, float> BrightnessCorrectionSet;



        public APIServer(string ip, int port, List<IAnimation> animations)
        {
            _ip = ip;
            _port = port;
            _animations = animations;
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
            Console.Out.WriteLine("APIServer : Received accept callback");
            // Get the socket that handles the client request.  
            ISocketConnection listener = (ISocketConnection)ar.AsyncState;

            lock (_isShuttingDownLock)
            {
                if (_isShuttingDown)
                {
                    Console.WriteLine("APIServer: ignored new client callback as the server is shutting down.");
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
            Console.WriteLine($"APIServer: Client {client.ID} disconnected, {e.SocketErrorCode}"); // TODO add intended disconnect, not just on SocketException
            ConnectedClients.Remove(client);
            DisposeClient(client);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs<MessageType> e)
        {
            Console.Out.WriteLine($"[API-IN] {e.MessageType}");
            switch (e.MessageType)
            {
                case MessageType.None:
                    break;
                case MessageType.GetAvailableStoryboards:
                    ParseGetAvailableStoryboards();
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
                case MessageType.GetTimeUnitsPerFrame:
                    ParseGetTimeUnitsPerFrame(e.Message);
                    break;
                case MessageType.SetTimeUnitsPerFrame:
                    ParseSetTimeUnitsPerFrame(e.Message);
                    break;
                case MessageType.GetRgbFade:
                    ParseGetRgbFadeMessage(e.Message);
                    break;
                case MessageType.SetRgbFade:
                    ParseSetRgbFadeMessage(e.Message);
                    break;
                case MessageType.GetBrightnessCorrection:
                    ParseGetBrightnessCorrectionMessage(e.Message);
                    break;
                case MessageType.SetBrightnessCorrection:
                    ParseSetBrightnessCorrectionMessage(e.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"APIServer: Unknown message type {e}");
            }
        }

        private void ParseGetAvailableStoryboards()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);
            StoryboardSerializer serializer = new StoryboardSerializer();

            // TODO also send playlists
            serializer.Save(_animations.OfType<Storyboard>().ToArray(), streamWriter);
           
            // send
            streamWriter.Flush();
            memoryStream.Flush();
            // TODO Inefficient. Right now we are converting from string to byte to string to byte....
            // TODO Encapsulate header size stuff
            string message = Encoding.ASCII.GetString(memoryStream.ToArray());
            byte[][] bytes = StringProtocol.Serialize(message, BUFFER_SIZE - PacketProtocol<MessageType>.HEADER_SIZE);
            for (int i = 0; i < bytes.Length; i++)
            {
                Send(MessageType.GetAvailableStoryboards, bytes[i]);
            }
        }

        private void ParseGetTimeUnitsPerFrame(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data,0);
                if (TimeUnitsPerFrameRequested != null)
                {
                    int timeUnitsPerFrame = TimeUnitsPerFrameRequested(animationIndex);
                    byte[] returnData = new byte[8];
                    BitConverter.GetBytes(animationIndex).CopyTo(returnData,0);
                    BitConverter.GetBytes(timeUnitsPerFrame).CopyTo(returnData,4);
                    Send(MessageType.GetTimeUnitsPerFrame, returnData);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to retrieve the TimeUnitsPerFrame.");
                Console.Out.WriteLine(e);
                return;
            }
        }

        private void ParseSetTimeUnitsPerFrame(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data,0);
                int timeUnitsPerFrame    = BitConverter.ToInt32(data,4);
                if (TimeUnitsPerFrameSet != null)
                {
                    TimeUnitsPerFrameSet.Invoke(animationIndex,timeUnitsPerFrame);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to set the TimeUnitsPerFrame.");
                Console.Out.WriteLine(e);
                return;
            }
        }

        private void ParseGetRgbFadeMessage(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data,0);
                if (RgbFadeRequested != null)
                {
                    float[] rgbFade = RgbFadeRequested.Invoke(animationIndex);
                    byte[] returnData = new byte[16];
                    BitConverter.GetBytes(animationIndex).CopyTo(returnData, 0);
                    BitConverter.GetBytes(rgbFade[0]).CopyTo(returnData,4);
                    BitConverter.GetBytes(rgbFade[1]).CopyTo(returnData,8);
                    BitConverter.GetBytes(rgbFade[2]).CopyTo(returnData,12);
                    Send(MessageType.GetRgbFade, returnData);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to retrieve the RgbFade.");
                Console.Out.WriteLine(e);
                return;
            }
        }

        private void ParseSetRgbFadeMessage(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data, 0);
                float[] rgbFade = new float[]
                {
                    BitConverter.ToSingle(data,4),
                    BitConverter.ToSingle(data,8),
                    BitConverter.ToSingle(data,12)
                };
                if (RgbFadeSet != null)
                {
                    RgbFadeSet.Invoke(animationIndex, rgbFade);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to set the RgbFade.");
                Console.Out.WriteLine(e);
                return;
            }
        }

        private void ParseGetBrightnessCorrectionMessage(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data,0);
                if (BrightnessCorrectionRequested != null)
                {
                    float brightnessCorrection = BrightnessCorrectionRequested(animationIndex);
                    byte[] returnData = new byte[8];
                    BitConverter.GetBytes(animationIndex).CopyTo(returnData, 0);
                    BitConverter.GetBytes(brightnessCorrection).CopyTo(returnData, 4);
                    Send(MessageType.GetBrightnessCorrection, returnData);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to retrieve the brightness correction.");
                Console.Out.WriteLine(e);
                return;
            }
        }
        private void ParseSetBrightnessCorrectionMessage(byte[] data)
        {
            try
            {
                int animationIndex = BitConverter.ToInt32(data,0);
                float brightnessCorrection = BitConverter.ToSingle(data, 4);
                if (BrightnessCorrectionSet != null)
                {
                    BrightnessCorrectionSet(animationIndex, brightnessCorrection);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("APIServer: Failed to set the brightness correction.");
                Console.Out.WriteLine(e);
                return;
            }
        }

        private void ParseStartStoryboardMessage(byte[] data)
        {
            if (_stringProtocol.TryDeserialize(data, out string storyboardAsYaml))
            {
                // Reset StringProtocol
                _stringProtocol = new StringProtocol();

                StoryboardSerializer storyboardSerializer = new StoryboardSerializer();
                Storyboard storyboard = null;
                try
                {
                    storyboard =
                        storyboardSerializer.Load(
                            new StreamReader(
                                new MemoryStream(
                                    Encoding.ASCII.GetBytes(
                                        storyboardAsYaml)))); // TODO te stringProtocol should should return the byte[] . Now we convert from bytes -> string -> bytes.
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("APIServer: Failed to start storyboard.");
                    Console.Out.WriteLine(e);
                    return;
                }

                // Bubble up
                EventHandler<IAnimation> handler = StartAnimation;
                if (handler != null)
                {
                    handler.Invoke(this, storyboard);
                }
            }
        }

        private void ParseStartPreloadedStoryboardMessage(byte[] data)
        {
            if (_stringProtocol.TryDeserialize(data, out string animationName))
            {
                // Reset StringProtocol
                _stringProtocol = new StringProtocol();

                // Get the storyboard
                IAnimation animation = _animations.FirstOrDefault(x => x.Name == animationName);
                if (animation == null)
                {
                    Console.Out.WriteLine($"API: failed to start animation. Animation {animationName} does not exist");
                    return;
                }

                // Bubble up
                EventHandler <IAnimation> handler = StartAnimation;
                if (handler != null)
                {
                    handler.Invoke(this,animation);
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

        private void Send(MessageType type, byte[] data)
        {
            // Assume there is only one client.
            ConnectedClients[0].Send(type,data);
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
                Console.WriteLine("APIServer: Listening socket was already disconnected.");
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
