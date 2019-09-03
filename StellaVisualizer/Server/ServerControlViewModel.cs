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
        private readonly int _pixelsPerRow;
        private APIServer _apiServer;
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
            _stellaServer = new StellaServer(viewmodel.ConfigurationFile, "192.168.1.110", 20055, 20060, memoryServer, new AnimatorFactory(bitmapRepository));
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
                _stellaServer.Animator.TransformationController.SetBrightnessCorrection(brightnessCorrection);
                return;
            }

            _stellaServer.Animator.TransformationController.SetBrightnessCorrection(brightnessCorrection, animationIndex);
        }

        private void ApiServerOnRgbFadeSet(int animationIndex, float[] rgbFade)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(rgbFade);
                return;
            }

            _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(rgbFade, animationIndex);
        }

        private void ApiServerOnFrameWaitMsSet(int animationIndex, int frameWaitMs)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.TransformationController.SetFrameWaitMs(frameWaitMs);
                return;
            }

            _stellaServer.Animator.TransformationController.SetFrameWaitMs(frameWaitMs, animationIndex);
        }

        private float ApiServerOnBrightnessCorrectionRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation.AnimationsTransformationSettings[animationIndex].BrightnessCorrection;
        }

        private float[] ApiServerOnRgbFadeRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation
                .AnimationsTransformationSettings[animationIndex].RgbFadeCorrection;
        }

        private int ApiServerOnFrameWaitMsRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation
                .AnimationsTransformationSettings[animationIndex].FrameWaitMs;
        }


        private void AddBitmapAnimations(List<Storyboard> storyboards, string bitmapDirectory)  // TODO this function could use some recursion
        {
            DirectoryInfo directory = new DirectoryInfo(bitmapDirectory);

            // Iterate folders in directory
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                foreach (FileInfo fileInfo in subDirectory.GetFiles())
                {
                    if (fileInfo.Extension == ".png")
                    {
                        AddBitmapAnimation(storyboards, Path.Combine(subDirectory.Name, Path.GetFileNameWithoutExtension(fileInfo.Name)));
                    }
                }
            }

            // Iterate files in bitmap dir
            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                if (fileInfo.Extension == ".png")
                {
                    AddBitmapAnimation(storyboards, Path.GetFileNameWithoutExtension(fileInfo.Name));
                }
            }
        }

        private void AddBitmapAnimation(List<Storyboard> storyboards, string name)
        {
            Storyboard sb = new Storyboard();
            sb.Name = name;


            if (name.Contains("Full_Setup"))
            {
                sb.Name = name;
                sb.AnimationSettings = new IAnimationSettings[]
                {
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = 1440,
                            Wraps = true
                        }
                };
            }
            else
            {
                sb.AnimationSettings = new IAnimationSettings[]
                {
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        Wraps = true
                    },
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        StartIndex = _pixelsPerRow,
                        Wraps = true
                    },
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        StartIndex = _pixelsPerRow * 2,
                        Wraps = true
                    },
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        StartIndex = _pixelsPerRow * 3,
                        Wraps = true
                    },
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        StartIndex = _pixelsPerRow * 4,
                        Wraps = true
                    },
                    new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StripLength = _pixelsPerRow,
                        StartIndex = _pixelsPerRow * 5,
                        Wraps = true
                    },
                };
            }
        
            storyboards.Add(sb);

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
