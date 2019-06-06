﻿using System;
using System.Collections.Generic;
using System.IO;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Network;
using StellaServerLib.Serialization.Mapping;

namespace StellaServerLib
{
    public class StellaServer
    {
        private readonly string _mappingFilePath;
        private readonly string _ip;
        private readonly int _port;

        private List<PiMaskItem> _mask;
        private Server _server;


        public StellaServer(string mappingFilePath, string ip, int port)
        {
            _mappingFilePath = mappingFilePath;
            _ip = ip;
            _port = port;
        }

        public void Start()
        {
            // Read mapping
            _mask = LoadMask(_mappingFilePath);
            // Start Server
            _server = StartServer(_ip, _port);
        }

        private List<PiMaskItem> LoadMask(string mappingFilePath)
        {
            try
            {
                // Read the piMappings from file
                MappingLoader mappingLoader = new MappingLoader();
                List<PiMapping> piMappings = mappingLoader.Load(new StreamReader(mappingFilePath));

                // Convert them to a mask
                PiMaskCalculator piMaskCalculator = new PiMaskCalculator(piMappings);
                return piMaskCalculator.Calculate();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load mask.", e);
            }
        }

        private Server StartServer(string ip, int port)
        {
            try
            {
                Server server = new Server(ip, port);
                server.Start();
                return server;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start the server.");
            }
        }
    }
}
