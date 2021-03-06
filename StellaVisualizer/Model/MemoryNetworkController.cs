﻿using System;
using StellaClientLib;
using StellaClientLib.Serialization;
using StellaLib.Network;
using StellaVisualizer.Model.Client;
using StellaVisualizer.Model.Server;

namespace StellaVisualizer.Model
{
    public class MemoryNetworkController
    {
        private StellaClient[] _clients;
        private MemoryStellaServer[] _memoryStellaServers;

        private MemoryServer _memoryServer;
        
        public MemoryNetworkController(MemoryLedStrip[] ledstrips, int numberOfPixels, byte brightness)
        {
            if (_memoryStellaServers != null)
            {
                foreach (MemoryStellaServer memoryStellaServer in _memoryStellaServers)
                {
                    memoryStellaServer.MessageSend -= MemoryStellaServer_OnMessageSend;
                }
            }

            int numberOfClients = ledstrips.Length;

            _clients = new StellaClient[numberOfClients];
            _memoryStellaServers = new MemoryStellaServer[numberOfClients];
            for (int i = 0; i < numberOfClients; i++)
            {
                // Network
                _memoryStellaServers[i] = new MemoryStellaServer();
                _memoryStellaServers[i].MessageSend += MemoryStellaServer_OnMessageSend;
                
                _clients[i] = new StellaClient(new Configuration(i, "192.168.1.110", 20055, 20060, numberOfPixels, 18, 10, brightness), _memoryStellaServers[i], ledstrips[i]);
                _clients[i].Start();
            }
        }


        public void SetServer(MemoryServer memoryServer)
        {
            if (_memoryServer != null)
            {
                _memoryServer.FrameSend -= MemoryServerOnFrameSend;
            }

            _memoryServer = memoryServer;
            _memoryServer.FrameSend += MemoryServerOnFrameSend;
        }

        private void MemoryServerOnFrameSend(object sender, MessageSendEventArgs e)
        {
            // Server is sending a frame. Fake a send to the client
            if (_memoryStellaServers.Length > e.ID)
            {
                _memoryStellaServers[e.ID].OnRenderFrameReceived(e.frame);
            }
        }

        private void MemoryStellaServer_OnMessageSend(object sender, MessageType e)
        {
            // A client is sending a message
            Console.Out.WriteLine($"Client is sending message of type {e.ToString()} to the server.");
        }
    }
}
