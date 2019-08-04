using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;
using StellaTestSuite.Model;

namespace StellaTestSuite.Server
{
    public class ServerControlViewModel : INotifyPropertyChanged
    {
        private readonly MemoryNetworkController _memoryNetworkController;
        public ServerConfigurationViewModel ServerConfigurationViewModel { get; set; }

        public ServerControlPanelViewModel ServerControlPanelViewModel { get; set; }
        
        public ServerControlViewModel(MemoryNetworkController memoryNetworkController)
        {
            _memoryNetworkController = memoryNetworkController;
            ServerConfigurationViewModel = new ServerConfigurationViewModel();
            ServerConfigurationViewModel.ApplyRequested += ServerConfigurationViewModel_OnApplyRequested;
        }

        private void ServerConfigurationViewModel_OnApplyRequested(object sender, EventArgs e)
        {
            ServerConfigurationViewModel viewmodel = sender as ServerConfigurationViewModel;;

            // Start Repositories
            StoryboardRepository storyboardRepository = new StoryboardRepository(viewmodel.StoryboardDirectory);
            BitmapRepository bitmapRepository = new BitmapRepository(viewmodel.BitmapDirectory);

            // Load animations from disc
            List<Storyboard> storyboards = storyboardRepository.LoadStoryboards();
            if (storyboards.Count < 1)
            {
               throw new ArgumentException("No storyboards found!");
            }

            // Add animations on the images in the bitmap directory
            AddBitmapAnimations(storyboards, viewmodel.BitmapDirectory);

            // Store in the ServerControlPanelViewModel
            ServerControlPanelViewModel = new ServerControlPanelViewModel(storyboards);
            ServerControlPanelViewModel.StartStoryboardRequested += ServerControlPanelViewModel_OnStartStoryboardRequested;

            // Start a new Server
            _memoryNetworkController.StartServer(viewmodel.ConfigurationFile, viewmodel.BitmapDirectory);
        }

        private void ServerControlPanelViewModel_OnStartStoryboardRequested(object sender, Storyboard e)
        {
            _memoryNetworkController.StellaServer.StartStoryboard(e);
        }

        private static void AddBitmapAnimations(List<Storyboard> storyboards, string bitmapDirectory)
        {
            DirectoryInfo directory = new DirectoryInfo(bitmapDirectory);
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                if (fileInfo.Extension == ".png")
                {
                    Storyboard sb = new Storyboard();
                    string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    sb.Name = name;

                    if (name.Contains("3600"))
                    {
                        sb.Name = name;
                        // Assume we have 2 pi's, each with 1 line of 240 pixels
                        sb.AnimationSettings = new IAnimationSettings[]
                        {
                            new BitmapAnimationSettings
                            {
                                FrameWaitMs = 10,
                                ImageName = name,
                                StripLength = 3600,
                                Wraps = true
                            }
                        };
                    }
                    else
                    {
                        // Assume we have 2 pi's, each with 1 line of 240 pixels
                        sb.AnimationSettings = new IAnimationSettings[]
                        {
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            Wraps = true
                        },
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            StartIndex = 600,
                            Wraps = true
                        },
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            StartIndex = 1200,
                            Wraps = true
                        },
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            StartIndex = 1800,
                            Wraps = true
                        },
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            StartIndex = 2400,
                            Wraps = true
                        },
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 600,
                            StartIndex = 3000,
                            Wraps = true
                        },
                        };
                    }

                    storyboards.Add(sb);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
