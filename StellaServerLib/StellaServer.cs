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
        private readonly int _udpPort;
        private readonly AnimatorCreation _animatorCreation;

        private List<PiMaskItem> _mask;
        private int[] _stripLengthPerPi;
        private IServer _server;
        private readonly IServerFactory _serverFactory;
        private ClientController _clientController;

        public IAnimator Animator { get; private set; }

        public StellaServer(string mappingFilePath, string ip, int port, int udpPort, IServerFactory serverFactory, AnimatorCreation animatorCreation)
        {
            _mappingFilePath = mappingFilePath;
            _ip = ip;
            _port = port;
            _udpPort = udpPort;
            _serverFactory = serverFactory;
            _animatorCreation = animatorCreation;
        }

        public void Start()
        {
            // Read mapping
            _mask = LoadMask(_mappingFilePath);
            // Start Server
            _server = StartServer(_ip, _port, _udpPort, _serverFactory);
            // Start ClientController
            _clientController = StartClientController(_server);
        }

        public void StartStoryboard(Storyboard storyboard)
        {
            Console.Out.WriteLine($"Starting storyboard {storyboard.Name}");

            try
            {
                Animator = _animatorCreation.Create(storyboard, _stripLengthPerPi, _mask);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create new animator.",e);
            }

            _clientController.StartAnimation(Animator, DateTime.Now + TimeSpan.FromMilliseconds(200)); // TODO variable startAT

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
                return piMaskCalculator.Calculate(out _stripLengthPerPi);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load mask.", e);
            }
        }

        private IServer StartServer(string ip, int port, int udpPort, IServerFactory serverFactory)
        {
            Console.Out.WriteLine($"Starting server on {ip}:{port}");
            try
            {
                IServer server = serverFactory.Create(ip, port, udpPort);
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
                clientController.Run();
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
