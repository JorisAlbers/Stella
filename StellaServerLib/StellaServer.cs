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
        private readonly int _millisecondsPerTimeUnit;
        private readonly BitmapRepository _bitmapRepository;

        private AnimatorCreator _animatorCreator;
        private IServer _server;
        private ClientController _clientController;

        private int _loadingAnimation;

        public IAnimator Animator { get; private set; }

        public StellaServer(string mappingFilePath, string ip, int port, int udpPort, int millisecondsPerTimeUnit, BitmapRepository bitmapRepository, IServer server)
        {
            _mappingFilePath = mappingFilePath;
            _ip = ip;
            _port = port;
            _udpPort = udpPort;
            _millisecondsPerTimeUnit = millisecondsPerTimeUnit;
            _bitmapRepository = bitmapRepository;
            _server = server;
        }

        public void Start()
        {
            // Read mapping
            List<PiMaskItem> mask = LoadMask(_mappingFilePath, out int[] stripLengthPerPi);
            // Create animatorCreator
            _animatorCreator = new AnimatorCreator(new FrameProviderCreator(_bitmapRepository, _millisecondsPerTimeUnit), stripLengthPerPi, mask);

            // Start Server
            _server = StartServer(_ip, _port, _udpPort, _server);
            // Start ClientController
            _clientController = StartClientController(_server);
        }

        public void StartAnimation(IAnimation animation)
        {
            Console.Out.WriteLine($"Starting animation {animation.Name}");

            PlayList playList = animation as PlayList;
            if (animation is Storyboard storyboard)
            {
                playList = new PlayList(storyboard.Name, new PlayListItem[] { new PlayListItem(storyboard, 0) });
            }

            StartPlayList(playList);
        }
        
        private async void StartPlayList(PlayList playList)
        {
            IAnimator oldAnimator = Animator;
            try
            {
                // Check if we are already loading an animation. If so, skip.
                if (0 == Interlocked.Exchange(ref _loadingAnimation, 1))
                {
                    // Create the animation on a new task
                    Animator = await Task.Run(() => _animatorCreator.Create(playList));
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
                throw new Exception("Failed to create new animator.", e);
            }

            _clientController.StartAnimation(Animator, Environment.TickCount); // TODO variable startAT
            oldAnimator?.Dispose();
        }

        private List<PiMaskItem> LoadMask(string mappingFilePath, out int[] stripLengthPerPi)
        {
            try
            {
                // Read the piMappings from file
                MappingLoader mappingLoader = new MappingLoader();
                List<PiMapping> piMappings = mappingLoader.Load(new StreamReader(mappingFilePath));

                // Convert them to a mask
                PiMaskCalculator piMaskCalculator = new PiMaskCalculator(piMappings);
                return piMaskCalculator.Calculate(out stripLengthPerPi);
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
