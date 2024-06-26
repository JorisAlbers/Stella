using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Animation;
using StellaServerLib.Network;
using StellaServerLib.Serialization.Mapping;

namespace StellaServerLib
{
    public class StellaServer : ReactiveObject, IDisposable
    {
        private const int _LEDS_PER_TUBE = 120;
        private readonly string _ip;
        private readonly int _port;
        private readonly int _udpPort;
        private int _remoteUdpPort;

        private readonly int _millisecondsPerTimeUnit;

        private AnimatorCreator _animatorCreator;
        private IServer _server;
        private ClientController _clientController;

        private int _loadingAnimation;
        private readonly int _maximumFrameRate;

        [Reactive] public IAnimator Animator { get; private set; }

        public event EventHandler<ClientStatusChangedEventArgs> ClientStatusChanged;

        public StellaServer(string ip, int port, int udpPort,int remoteUdpPort, int millisecondsPerTimeUnit,int maximumFrameRate, IServer server)
        {
            _ip = ip;
            _port = port;
            _udpPort = udpPort;
            _remoteUdpPort = remoteUdpPort;
            _millisecondsPerTimeUnit = millisecondsPerTimeUnit;
            _maximumFrameRate = maximumFrameRate;
            _server = server;
        }

        public void Start(MappingLoader.Mapping mapping, BitmapRepository bitmapRepository)
        {
             // Create animatorCreator
             var (piMaskItems, stripLengthPerPi, rows, columns) = mapping;
             _animatorCreator = new AnimatorCreator(new FrameProviderCreator(bitmapRepository, _millisecondsPerTimeUnit, rows, columns, _LEDS_PER_TUBE), stripLengthPerPi, piMaskItems);

            // Start Server
            _server = StartServer(_ip, _port, _udpPort, _remoteUdpPort,_server);
            // Start ClientController
            _clientController = StartClientController(_server, _maximumFrameRate, stripLengthPerPi.Length);
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
                    _clientController.StartAnimation(Animator);
                    oldAnimator?.Dispose();
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
        }

        private IServer StartServer(string ip, int port, int udpPort,int remoteUdpPort, IServer server)
        {
            Console.Out.WriteLine($"Starting server on {ip}:{port}");
            try
            {
                server.ClientChanged += ServerOnClientChanged;
                server.Start(ip, port, udpPort, remoteUdpPort);
                return server;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to start the server.",e);
            }
        }

        private void ServerOnClientChanged(object sender, ClientStatusChangedEventArgs e)
        {
            ClientStatusChanged?.Invoke(sender, e);
        }

        private ClientController StartClientController(IServer server, int maximumFrameRate, int numberOfClients)
        {
            try
            {
                ClientController clientController = new ClientController(server, maximumFrameRate, numberOfClients);
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
