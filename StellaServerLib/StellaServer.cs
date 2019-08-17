using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
        private ClientController _clientController;

        private int _loadingAnimation;

        public IAnimator Animator { get; private set; }

        public StellaServer(string mappingFilePath, string ip, int port, int udpPort, IServer server, AnimatorCreation animatorCreation)
        {
            _mappingFilePath = mappingFilePath;
            _ip = ip;
            _port = port;
            _udpPort = udpPort;
            _server = server;
            _animatorCreation = animatorCreation;
        }

        public void Start()
        {
            // Read mapping
            _mask = LoadMask(_mappingFilePath);
            // Start Server
            _server = StartServer(_ip, _port, _udpPort, _server);
            // Start ClientController
            _clientController = StartClientController(_server);
        }

        public async void StartStoryboard(Storyboard storyboard)
        {
            Console.Out.WriteLine($"Starting storyboard {storyboard.Name}");

            try
            {
                // Check if we are already loading an animation. If so, skip.
                if (0 == Interlocked.Exchange(ref _loadingAnimation, 1))
                {
                    // Create the animation on a new task
                    Animator = await Task.Factory.StartNew(() => _animatorCreation.Create(storyboard, _stripLengthPerPi, _mask)); 
                    // Release the lock
                    Interlocked.Exchange(ref _loadingAnimation, 0);
                }
                else
                {
                    Console.Out.WriteLine("Failed to create a new animation, we are already loading one");
                    return;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create new animator.",e);
            }

            _clientController.StartAnimation(Animator, Environment.TickCount); // TODO variable startAT

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

        private IServer StartServer(string ip, int port, int udpPort, IServer server)
        {
            Console.Out.WriteLine($"Starting server on {ip}:{port}");
            try
            {
                server.Start(ip, port, udpPort);
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
