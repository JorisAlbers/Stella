using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        private APIServer _apiServer;
        private StellaServer _stellaServer;


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

            // Start a new Server
            MemoryServer memoryServer = new MemoryServer();
            _memoryNetworkController.SetServer(memoryServer);
            _stellaServer = new StellaServer(viewmodel.ConfigurationFile, "192.168.1.110", 20055, 20060, memoryServer, new AnimatorCreation(bitmapRepository));
            _stellaServer.Start();

            // Store in the ServerControlPanelViewModel
            ServerControlPanelViewModel = new ServerControlPanelViewModel(_stellaServer, storyboards);
            ServerControlPanelViewModel.StartStoryboardRequested += ServerControlPanelViewModel_OnStartStoryboardRequested;
            

            // Start API server
            if (!string.IsNullOrWhiteSpace(viewmodel.ApiServerIpAddress))
            {
                try
                {
                    _apiServer = new APIServer(viewmodel.ApiServerIpAddress, 20060, storyboards);
                    _apiServer.FrameWaitMsRequested += ApiServerOnFrameWaitMsRequested;
                    _apiServer.FrameWaitMsSet += ApiServerOnFrameWaitMsSet;
                    _apiServer.RgbFadeRequested += ApiServerOnRgbFadeRequested;
                    _apiServer.RgbFadeSet += ApiServerOnRgbFadeSet;
                    _apiServer.BrightnessCorrectionRequested += ApiServerOnBrightnessCorrectionRequested;
                    _apiServer.BrightnessCorrectionSet += ApiServerOnBrightnessCorrectionSet;
                    _apiServer.StartStoryboard += (s, storyboard) => _stellaServer.StartStoryboard(storyboard);
                    _apiServer.BitmapReceived += (s, eventArgs) =>
                    {
                        if (bitmapRepository.BitmapExists(eventArgs.Name))
                        {
                            throw new Exception("Failed to store bitmap. A bitmap with the name {eventArgs.Name} already exists.");
                        }

                        bitmapRepository.Save(eventArgs.Bitmap, eventArgs.Name);
                    };

                    _apiServer.Start();
                }
                catch (Exception exception)
                {
                    Console.Out.WriteLine("An exception occured when starting the APIServer.");
                    throw;
                }
            }
        }

        private void ServerControlPanelViewModel_OnStartStoryboardRequested(object sender, Storyboard e)
        {
            _stellaServer.StartStoryboard(e);
        }
        private void ApiServerOnBrightnessCorrectionSet(int animationIndex, float brightnessCorrection)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.AnimationTransformation.SetBrightnessCorrection(brightnessCorrection);
                return;
            }

            _stellaServer.Animator.AnimationTransformation.SetBrightnessCorrection(brightnessCorrection, animationIndex);
        }

        private void ApiServerOnRgbFadeSet(int animationIndex, float[] rgbFade)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.AnimationTransformation.SetRgbFadeCorrection(rgbFade);
                return;
            }

            _stellaServer.Animator.AnimationTransformation.SetRgbFadeCorrection(rgbFade, animationIndex);
        }

        private void ApiServerOnFrameWaitMsSet(int animationIndex, int frameWaitMs)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.AnimationTransformation.SetFrameWaitMs(frameWaitMs);
                return;
            }

            _stellaServer.Animator.AnimationTransformation.SetFrameWaitMs(frameWaitMs, animationIndex);
        }

        private float ApiServerOnBrightnessCorrectionRequested(int animationIndex)
        {
            return _stellaServer.Animator.AnimationTransformation.GetBrightnessCorrection(animationIndex);
        }

        private float[] ApiServerOnRgbFadeRequested(int animationIndex)
        {
            return _stellaServer.Animator.AnimationTransformation.GetRgbFadeCorrection(animationIndex);
        }

        private int ApiServerOnFrameWaitMsRequested(int animationIndex)
        {
            return _stellaServer.Animator.AnimationTransformation.GetFrameWaitMs(animationIndex);
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
