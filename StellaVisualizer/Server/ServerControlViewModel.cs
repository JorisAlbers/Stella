using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using StellaServerAPI;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Serialization.Animation;
using StellaServerLib.Serialization.Mapping;
using StellaServerLib.VideoMapping;
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

           
            // Start a new Server
            MemoryServer memoryServer = new MemoryServer();
            _memoryNetworkController.SetServer(memoryServer);
            _stellaServer = new StellaServer("192.168.1.110", 20055, 20060,20060, 1, 60,  memoryServer);

            // Read mapping
            MappingLoader mappingLoader = new MappingLoader();
            using var reader = new StreamReader(viewmodel.ConfigurationFile);
            var mapping = mappingLoader.Load(reader);

            // Add animations on the images in the bitmap directory

            string resizedRepositoryPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "StellaServer", "Bitmaps",
                    $"{mapping.Columns}");
            BitmapRepository resizedBitmapRepository = new BitmapRepository(new FileSystem(), resizedRepositoryPath);

            _stellaServer.Start(mapping, resizedBitmapRepository);

            BitmapStoryboardCreator bitmapStoryboardCreator = new BitmapStoryboardCreator(bitmapRepository, resizedBitmapRepository, mapping.Rows,mapping.Columns, 120);
            storyboards.AddRange(bitmapStoryboardCreator.Create());


            // Video mapping
            if (!string.IsNullOrWhiteSpace(viewmodel.VideoRepository))
            {
                VideoMappingStoryBoardCreator videoMappingStoryBoardCreator =
                    new VideoMappingStoryBoardCreator(viewmodel.VideoRepository, resizedBitmapRepository, mapping.Rows, mapping.Columns,
                        120);

                storyboards.AddRange(videoMappingStoryBoardCreator.Create());
            }
           

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
