using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using StellaServerAPI;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;
using StellaVisualizer.Model;
using StellaVisualizer.Model.Server;

namespace StellaVisualizer.Server
{
    public class ServerControlViewModel : INotifyPropertyChanged
    {
        private readonly MemoryNetworkController _memoryNetworkController;
        private readonly int _pixelsPerRow;
        private StellaServer _stellaServer;
        private int _totalNumberOfPixels;


        public ServerConfigurationViewModel ServerConfigurationViewModel { get; set; }

        public ServerControlPanelViewModel ServerControlPanelViewModel { get; set; }



        public ServerControlViewModel(MemoryNetworkController memoryNetworkController, int pixelsPerRow, int totalNumberOfPixels)
        {
            _memoryNetworkController = memoryNetworkController;
            _pixelsPerRow = pixelsPerRow;
            _totalNumberOfPixels = totalNumberOfPixels;
            ServerConfigurationViewModel = new ServerConfigurationViewModel();
            ServerConfigurationViewModel.ApplyRequested += ServerConfigurationViewModel_OnApplyRequested;
        }

        private void ServerConfigurationViewModel_OnApplyRequested(object sender, EventArgs e)
        {
            ServerConfigurationViewModel viewmodel = sender as ServerConfigurationViewModel;;

            // Start Repositories
            StoryboardRepository storyboardRepository = new StoryboardRepository(viewmodel.StoryboardDirectory);
            BitmapRepository bitmapRepository = new BitmapRepository(new FileSystem(), viewmodel.BitmapDirectory);

            // Load animations from disc
            List<Storyboard> storyboards = storyboardRepository.LoadStoryboards();
            if (storyboards.Count < 1)
            {
               throw new ArgumentException("No storyboards found!");
            }

            // Add animations on the images in the bitmap directory
            BitmapStoryboardCreator bitmapStoryboardCreator = new BitmapStoryboardCreator(bitmapRepository, _pixelsPerRow,3,2);
            storyboards.AddRange(bitmapStoryboardCreator.Create());


            // Start a new Server
            MemoryServer memoryServer = new MemoryServer();
            _memoryNetworkController.SetServer(memoryServer);
            _stellaServer = new StellaServer(viewmodel.ConfigurationFile, 20055, 20060,20060, 1, 60, bitmapRepository, memoryServer);
            _stellaServer.Start();

            List<IAnimation> animations = storyboards.Cast<IAnimation>().ToList();
            // Create play lists
            animations.Add(PlaylistCreator.Create("All combined", storyboards, 5));
            animations.AddRange(PlaylistCreator.CreateFromCategory(storyboards, 5));

            // Store in the ServerControlPanelViewModel
            ServerControlPanelViewModel = new ServerControlPanelViewModel(_stellaServer, animations);
            ServerControlPanelViewModel.StartAnimationRequested += ServerControlPanelViewModelOnStartAnimationRequested;
        }

        private void ServerControlPanelViewModelOnStartAnimationRequested(object sender, IAnimation e)
        {
            _stellaServer.StartAnimation(e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
