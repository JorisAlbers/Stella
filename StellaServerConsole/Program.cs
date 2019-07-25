using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StellaServerAPI;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerConsole
{
    class Program
    {
        private static StellaServer _stellaServer;
        private static APIServer _apiServer;
        private static BitmapRepository _bitmapRepository;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Starting StellaServer");

            // Parse args
            string mappingFilePath = null;
            string storyboardDirPath = null;
            string ip = null;
            int port = 0, udpPort = 0;
            string apiIp = null;
            int apiPort= 0;
            string bitmapDirectory = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    // arg is a flag
                    switch (args[i])
                    {
                        case "-h":
                            OutputHelp();
                            return;
                        case "-m":
                            mappingFilePath = args[++i];
                            break;
                        case "-s":
                            storyboardDirPath = args[++i];
                            break;
                        case "-ip":
                            ip = args[++i];
                            break;
                        case "-port":
                            port = int.Parse(args[++i]);
                            break;
                        case "-udp_port":
                            udpPort = int.Parse(args[++i]);
                            break;
                        case "-api_ip":
                            apiIp = args[++i];
                            break;
                        case "-api_port":
                            apiPort = int.Parse(args[++i]);
                            break;
                        case "-b":
                            bitmapDirectory = args[++i];
                            break;

                        default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }
            if(!ValidateCommandLineArguments(mappingFilePath,ip,port, udpPort,storyboardDirPath, apiIp, apiPort, bitmapDirectory))
            {
                return;
            }

            // Start Repositories
            StoryboardRepository storyboardRepository = new StoryboardRepository(storyboardDirPath);
            _bitmapRepository = new BitmapRepository(bitmapDirectory);

            // Load animations from disc
            List<Storyboard> storyboards = storyboardRepository.LoadStoryboards();
            if (storyboards.Count < 1)
            {
                Console.Out.WriteLine("No storyboards found!");
                return;
            }

            // Add animations on the images in the bitmap directory
            AddBitmapAnimations(storyboards, bitmapDirectory);

            string[] storyboardNames = storyboards.Select(x => x.Name).ToArray();

            // Start stellaServer
            _stellaServer = new StellaServer(mappingFilePath, ip, port,udpPort , new AnimatorCreation(_bitmapRepository));

            try
            {
                _stellaServer.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("An exception occured when starting StellaServer.");
                OutputError(e);
                return;
            }

            // Start serverAPI if requested
            if (!string.IsNullOrWhiteSpace(apiIp))
            {
                try
                {
                    _apiServer = new APIServer(apiIp, apiPort, storyboards);
                    _apiServer.FrameWaitMsRequested += ApiServerOnFrameWaitMsRequested;
                    _apiServer.FrameWaitMsSet += ApiServerOnFrameWaitMsSet;
                    _apiServer.RgbFadeRequested += ApiServerOnRgbFadeRequested;
                    _apiServer.RgbFadeSet += ApiServerOnRgbFadeSet;
                    _apiServer.BrightnessCorrectionRequested += ApiServerOnBrightnessCorrectionRequested;
                    _apiServer.BrightnessCorrectionSet += ApiServerOnBrightnessCorrectionSet;
                    _apiServer.StartStoryboard += (sender, storyboard) => _stellaServer.StartStoryboard(storyboard);
                    _apiServer.BitmapReceived += (sender, eventArgs) =>
                    {
                        if (_bitmapRepository.BitmapExists(eventArgs.Name))
                        {
                            Console.Out.WriteLine(
                                "Failed to store bitmap. A bitmap with the name {eventArgs.Name} already exists.");
                            return;
                        }

                        _bitmapRepository.Save(eventArgs.Bitmap, eventArgs.Name);
                    };

                    _apiServer.Start();
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("An exception occured when starting the APIServer.");
                    OutputError(e);
                    return;
                }
            }

            while (true)
            {
                OutputMenu(storyboardNames);
                string input = Console.ReadLine();
                if (input == "q")
                {
                    _stellaServer.Dispose();
                    break;
                }

                if (input == "t")
                {
                    GetTransformationInput();
                    continue;
                }
                

                if (int.TryParse(input, out int storyboardToPlay))
                {
                    if (storyboardToPlay < 0 || storyboardToPlay > storyboards.Count -1)
                    {
                        Console.Out.WriteLine("Invalid animation");
                        continue;
                    }

                    try
                    {
                        _stellaServer.StartStoryboard(storyboards[storyboardToPlay]);
                    }
                    catch (Exception e)
                    {
                        Console.Out.WriteLine($"Failed to start storyboard.");
                        OutputError(e);
                    }
                }
            }

            Console.Out.WriteLine("End of StellaServer.");
        }

        private static void ApiServerOnBrightnessCorrectionSet(int animationIndex, float brightnessCorrection)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.SetBrightnessCorrection(brightnessCorrection);
                return;
            }

            _stellaServer.Animator.SetBrightnessCorrection(animationIndex, brightnessCorrection);
        }

        private static void ApiServerOnRgbFadeSet(int animationIndex, float[] rgbFade)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.SetRgbFadeCorrection(rgbFade);
                return;
            }

            _stellaServer.Animator.SetRgbFadeCorrection(animationIndex, rgbFade);
        }

        private static void ApiServerOnFrameWaitMsSet(int animationIndex, int frameWaitMs)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.SetFrameWaitMs(frameWaitMs);
                return;
            }

            _stellaServer.Animator.SetFrameWaitMs(animationIndex, frameWaitMs);
        }

        private static float ApiServerOnBrightnessCorrectionRequested(int animationIndex)
        {
            return _stellaServer.Animator.GetBrightnessCorrection(animationIndex);
        }

        private static float[] ApiServerOnRgbFadeRequested(int animationIndex)
        {
            return _stellaServer.Animator.GetRgbFadeCorrection(animationIndex);
        }

        private static int ApiServerOnFrameWaitMsRequested(int animationIndex)
        {
            return _stellaServer.Animator.GetFrameWaitMs(animationIndex);
        }

        private static void GetTransformationInput()
        {
            Console.Out.WriteLine("TRANSFORMATION MODE, q to quit");
            Console.Out.WriteLine("s           = speed");
            Console.Out.WriteLine("t           = brightness");
            Console.Out.WriteLine("r           = red color");
            Console.Out.WriteLine("g           = green color");
            Console.Out.WriteLine("b           = blue color");
            Console.Out.WriteLine("shift + key = lower");

            float brightnessCorrection = 0;
            float[] transformationFactor = new float[3]{0,0,0};

            ConsoleKeyInfo key;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Q)
            {
                // Animation speed
                if (key.KeyChar == 's' || key.KeyChar == 'S')
                {
                    int currentWaitms = _stellaServer.Animator.GetFrameWaitMs(0);
                    int newWaitMs;
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        // lower
                        newWaitMs = currentWaitms + 1;
                    }
                    else
                    {
                        // raise
                        newWaitMs = Math.Max(1, currentWaitms - 1);
                    }

                    _stellaServer.Animator.SetFrameWaitMs(newWaitMs);
                    Console.Out.WriteLine($"Speed set to {newWaitMs}");
                }

                // Brightness
                if (key.KeyChar == 't' || key.KeyChar == 'T')
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        // lower
                        brightnessCorrection -= 0.10f;
                        if (brightnessCorrection < -1)
                            brightnessCorrection = -1;
                    }
                    else
                    {
                        // raise
                        brightnessCorrection += 0.10f;
                        if (brightnessCorrection > 1)
                            brightnessCorrection = 1;
                    }
                    _stellaServer.Animator.SetBrightnessCorrection(brightnessCorrection);
                }

                // Color
                if (key.KeyChar == 'r' || key.KeyChar == 'R')
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        transformationFactor[0] -= 0.05f;
                        if (transformationFactor[0] < -1)
                            transformationFactor[0] = -1;
                    }
                    else
                    {
                        transformationFactor[0] += 0.05f;
                        if (transformationFactor[0] > 0)
                            transformationFactor[0] = 0;
                    }
                    _stellaServer.Animator.SetRgbFadeCorrection(transformationFactor);
                }
                if (key.KeyChar == 'g' || key.KeyChar == 'G')
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        transformationFactor[1] -= 0.05f;
                        if (transformationFactor[1] < -1)
                            transformationFactor[1] = -1;
                    }
                    else
                    {
                        transformationFactor[1] += 0.05f;
                        if (transformationFactor[1] > 0)
                            transformationFactor[1] = 0;
                    }
                    _stellaServer.Animator.SetRgbFadeCorrection(transformationFactor);
                }
                if (key.KeyChar == 'b' || key.KeyChar == 'B')
                {
                    if (key.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        transformationFactor[2] -= 0.05f;
                        if (transformationFactor[2] < -1)
                            transformationFactor[2] = -1;
                    }
                    else
                    {
                        transformationFactor[2] += 0.05f;
                        if (transformationFactor[2] > 0)
                            transformationFactor[2] = 0;
                    }
                    _stellaServer.Animator.SetRgbFadeCorrection(transformationFactor);
                }
               
            }
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

        static bool ValidateCommandLineArguments(string mappingFilePath, string ip, int port,int udpPort, string storyboardDirPath, string apiIp, int apiPort, string bitmapDirectory)
        {
            // TODO path and file exist validation
            if (mappingFilePath == null)
            {
                Console.Out.WriteLine("The mapping file must be set. Use -m <mapping_filepath>");
                return false;
            }

            if (port == 0)
            {
                Console.Out.WriteLine("The port must be set. Use -port <port value>");
                return false;
            }
            if (udpPort == 0)
            {
                Console.Out.WriteLine("The udp_port must be set. Use -udp_port <port value>");
                return false;
            }
            if (port == udpPort)
            {
                Console.Out.WriteLine("The port and the udp_port can't be identical.");
                return false;
            }

            if (ip == null)
            {
                Console.Out.WriteLine("The ip must be set. Use -ip <ip value>");
                return false;
            }

            if (apiPort == 0 && !string.IsNullOrWhiteSpace(apiIp))
            {
                Console.Out.WriteLine("The apiPort must be set. Use -api_port <port value>");
                return false;
            }

            if (string.IsNullOrWhiteSpace(apiIp) && apiPort != 0)
            {
                Console.Out.WriteLine("The apiIP must be set. Use -api_ip <ip value>");
                return false;
            }

            if (storyboardDirPath == null)
            {
                Console.Out.WriteLine("The storyboard directory path must be set. Use -s <dir path>");
                return false;
            }

            if (!Directory.Exists(storyboardDirPath))
            {
                Console.Out.WriteLine("The storyboard directory path does not point to an existing directory");
                return false;
            }

            if (!Directory.Exists(bitmapDirectory))
            {
                Console.Out.WriteLine("The bitmap directory path does not point to an existing directory");
                return false;
            }

            return true;
        }

        static void OutputHelp()
        {
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("To run: StellaServer -m <mapping_filepath>");
            Console.Out.WriteLine("Optional:");
            Console.Out.WriteLine("-b <bitmap_directory_path> : creates animations with bitmap drawers ");
            Console.Out.WriteLine("                             for each image in the dir.");
            Console.Out.WriteLine();
        }

        static void OutputMenu(string[] storyboardNames)
        {
            Console.Out.WriteLine("Exit program = q");
            Console.Out.WriteLine("Transformation mode = t");
            Console.Out.WriteLine("Start animation:");
            for (int i = 0; i < storyboardNames.Length; i++)
            {
                Console.Out.WriteLine($"{i} - {storyboardNames[i]}");
            }
            Console.Out.WriteLine();
        }

        static void OutputError(Exception e)
        {
            Console.Out.WriteLine(e.Message);
            Console.Out.WriteLine(e.StackTrace);
            if (e.InnerException != null)
            {
                Console.Out.WriteLine(e.InnerException.Message);
                Console.Out.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
