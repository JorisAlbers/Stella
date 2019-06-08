using System;
using System.Collections.Generic;
using System.IO;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Network;
using StellaServerLib.Serialization.Mapping;

namespace StellaServerLib
{
    public class StellaServer : IDisposable
    {
        private readonly string _mappingFilePath;
        private readonly string _ip;
        private readonly int _port;

        private List<PiMaskItem> _mask;
        private Server _server;
        private ClientController _clientController;


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
            // Start ClientController
            _clientController = StartClientController(_server);
        }

        public void StartStoryboard(Storyboard storyboard)
        {
            Console.Out.WriteLine($"Starting storyboard {storyboard.Name}");

            IAnimator animator;
            try
            {
                animator = AnimatorCreation.Create(storyboard, _mask, DateTime.Now);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create new animator.",e);
            }

            _clientController.StartAnimation(animator);

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
            Console.Out.WriteLine($"Starting server on {ip}:{port}");
            try
            {
                Server server = new Server(ip, port);
                server.Start();
                return server;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start the server.",e);
            }
        }

        private ClientController StartClientController(IServer server)
        {
            try
            {
                ClientController clientController = new ClientController(server);
                return clientController;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start the ClientController.",e);
            }
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
