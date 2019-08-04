using System;
using StellaClientLib;
using StellaClientLib.Serialization;
using StellaLib.Network;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaTestSuite.Model.Client;
using StellaTestSuite.Model.Server;

namespace StellaTestSuite.Model
{
    class MemoryNetworkController
    {
        private StellaClient[] _clients;
        private MemoryStellaServer[] _memoryStellaServers;

        private StellaServer _stellaServer;
        private MemoryServer _memoryServer;

        /// <summary>
        /// The server is sending a message to a client
        /// </summary>
        public event EventHandler<MessageSendEventArgs> FrameSend;

        public MemoryNetworkController(int numberOfClients, int numberOfPixels, int minimumFrameRate, byte brightness)
        {
            if (_memoryStellaServers != null)
            {
                foreach (MemoryStellaServer memoryStellaServer in _memoryStellaServers)
                {
                    memoryStellaServer.MessageSend -= MemoryStellaServer_OnMessageSend;
                }
            }

            _clients = new StellaClient[numberOfClients];
            _memoryStellaServers = new MemoryStellaServer[numberOfClients];
            for (int i = 0; i < numberOfClients; i++)
            {
                _memoryStellaServers[i] = new MemoryStellaServer();
                _memoryStellaServers[i].MessageSend += MemoryStellaServer_OnMessageSend;
                _clients[i] = new StellaClient(new Configuration(i, string.Empty, 0, 0, numberOfPixels, 0, 0, minimumFrameRate, brightness), _memoryStellaServers[i] );
            }
        }

        public void StartServer(string mappingFilePath, string bitmapDirectoryPath)
        {
            if (_memoryServer != null)
            {
                _memoryServer.FrameSend -= MemoryServerOnFrameSend;
            }

            _memoryServer = new MemoryServer();
            _memoryServer.FrameSend += MemoryServerOnFrameSend;
            _stellaServer = new StellaServer(mappingFilePath,string.Empty,0,0,_memoryServer,new AnimatorCreation(new BitmapRepository(bitmapDirectoryPath)));
            _stellaServer.Start();
        }

        private void MemoryServerOnFrameSend(object sender, MessageSendEventArgs e)
        {
            // Server is sending a frame. Bubble the event
            var eventHandler = FrameSend;
            if (eventHandler != null)
            {
                eventHandler.Invoke(sender,e);
            }
        }

        private void MemoryStellaServer_OnMessageSend(object sender, MessageType e)
        {
            // A client is sending a message
            Console.Out.WriteLine($"Client is sending message of type {e.ToString()} to the server.");
        }
    }
}
