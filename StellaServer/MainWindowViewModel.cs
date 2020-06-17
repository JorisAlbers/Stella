using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using StellaServer.ServerConfiguration;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ServerConfigurationViewModel SeverConfigurationViewModel { get; }

        private StellaServerLib.StellaServer _server;
        private BitmapRepository _bitmapRepository;


        public MainWindowViewModel()
        {
            SeverConfigurationViewModel = new ServerConfigurationViewModel();
            SeverConfigurationViewModel.SaveRequested += SeverConfigurationViewModel_OnSaveRequested;
        }

        private void SeverConfigurationViewModel_OnSaveRequested(object sender, EventArgs e)
        {
            string mappingFilePath = null;
            string storyboardDirPath = null;
            string ip = null;
            int port = 0, udpPort = 0;
            string apiIp = null;
            int apiPort = 0;
            int millisecondsPerTimeUnit = 1;
            string bitmapDirectory = null;


            // Start Repositories
            StoryboardRepository storyboardRepository = new StoryboardRepository(SeverConfigurationViewModel.StoryboardFolderPath);
            _bitmapRepository = new BitmapRepository(SeverConfigurationViewModel.BitmapFolderPath);

            // Load animations from disc
            List<Storyboard> storyboards = storyboardRepository.LoadStoryboards();
            if (storyboards.Count < 1)
            {
                Console.Out.WriteLine("No storyboards found!");
                return;
            }

            // Add animations on the images in the bitmap directory
            BitmapStoryboardCreator bitmapStoryboardCreator = new BitmapStoryboardCreator(new DirectoryInfo(SeverConfigurationViewModel.BitmapFolderPath), 360, 3, 2); // TODO get these magic variables from the mapping.
            bitmapStoryboardCreator.Create(storyboards);

            List<IAnimation> animations = storyboards.Cast<IAnimation>().ToList();

            // Create play lists
            animations.Add(PlaylistCreator.Create("All combined", storyboards, 120));
            animations.AddRange(PlaylistCreator.CreateFromCategory(storyboards, 120));

            string[] animationNames = animations.Select(x => x.Name).ToArray();

            // Start stellaServer
            _server = new StellaServerLib.StellaServer(SeverConfigurationViewModel.MappingFilePath, SeverConfigurationViewModel.IpAddress, SeverConfigurationViewModel.Port, SeverConfigurationViewModel.UdpPort, millisecondsPerTimeUnit, _bitmapRepository, new Server());

        }
    }
}
