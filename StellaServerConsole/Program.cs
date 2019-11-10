using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StellaServerAPI;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServerConsole
{
    class Program
    {
        private static StellaServer _stellaServer;
        private static APIServer _apiServer;
        private static BitmapRepository _bitmapRepository;
        
        static void Main(string[] args)
        {
            const int numberOfRequiredArguments = 6;
            if (args.Length < numberOfRequiredArguments)
            {
                Console.Out.WriteLine(_HELP_TEXT);
                return;
            }
            
            // Parse args
            string mappingFilePath = null;
            string storyboardDirPath = null;
            string ip = null;
            int port = 0, udpPort = 0;
            string apiIp = null;
            int apiPort= 0;
            int millisecondsPerTimeUnit = 1;
            string bitmapDirectory = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    // arg is a flag
                    switch (args[i])
                    {
                        case "-h":
                            Console.WriteLine(_HELP_TEXT);
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
                        case "-t":
                            millisecondsPerTimeUnit = int.Parse(args[++i]);
                            break;

                        default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }
            if(!ValidateCommandLineArguments(mappingFilePath,ip,port, udpPort,storyboardDirPath, apiIp, apiPort, bitmapDirectory, millisecondsPerTimeUnit))
            {
                return;
            }

            Console.WriteLine("Starting StellaServer");

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
            BitmapStoryboardCreator bitmapStoryboardCreator = new BitmapStoryboardCreator(new DirectoryInfo(bitmapDirectory),360,3,2); // TODO get these magic variables from the mapping.
            bitmapStoryboardCreator.Create(storyboards);
            
            List<IAnimation> animations = storyboards.Cast<IAnimation>().ToList();
            
            // Create play lists
            animations.Add(PlaylistCreator.Create("All combined", storyboards, 120));
            animations.AddRange(PlaylistCreator.CreateFromCategory(storyboards, 120));

            string[] animationNames = animations.Select(x => x.Name).ToArray();

            // Start stellaServer
            _stellaServer = new StellaServer(mappingFilePath, ip, port,udpPort , millisecondsPerTimeUnit, _bitmapRepository, new Server());

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
                    _apiServer = new APIServer(apiIp, apiPort, animations);
                    _apiServer.TimeUnitsPerFrameRequested += ApiServerOnTimeUnitsPerFrameRequested;
                    _apiServer.TimeUnitsPerFrameSet += ApiServerOnTimeUnitsPerFrameSet;
                    _apiServer.RgbFadeRequested += ApiServerOnRgbFadeRequested;
                    _apiServer.RgbFadeSet += ApiServerOnRgbFadeSet;
                    _apiServer.BrightnessCorrectionRequested += ApiServerOnBrightnessCorrectionRequested;
                    _apiServer.BrightnessCorrectionSet += ApiServerOnBrightnessCorrectionSet;
                    _apiServer.StartAnimation += (sender, storyboard) => _stellaServer.StartAnimation(storyboard);
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
                OutputMenu(animationNames);
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
                        _stellaServer.StartAnimation(storyboards[storyboardToPlay]);
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
                _stellaServer.Animator.TransformationController.SetBrightnessCorrection(brightnessCorrection);
                return;
            }

            _stellaServer.Animator.TransformationController.SetBrightnessCorrection(brightnessCorrection, animationIndex);
        }

        private static void ApiServerOnRgbFadeSet(int animationIndex, float[] rgbFade)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(rgbFade);
                return;
            }

            _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(rgbFade, animationIndex);
        }

        private static void ApiServerOnTimeUnitsPerFrameSet(int animationIndex, int timeUnitsPerFrame)
        {
            if (animationIndex == -1)
            {
                // Set for all
                _stellaServer.Animator.TransformationController.SetTimeUnitsPerFrame(timeUnitsPerFrame);
                return;
            }

            _stellaServer.Animator.TransformationController.SetTimeUnitsPerFrame(animationIndex, timeUnitsPerFrame);
        }

        private static float ApiServerOnBrightnessCorrectionRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation.AnimationsTransformationSettings[animationIndex].BrightnessCorrection;
        }

        private static float[] ApiServerOnRgbFadeRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation.AnimationsTransformationSettings[animationIndex].RgbFadeCorrection;
        }

        private static int ApiServerOnTimeUnitsPerFrameRequested(int animationIndex)
        {
            return _stellaServer.Animator.TransformationController.AnimationTransformation.AnimationsTransformationSettings[animationIndex].TimeUnitsPerFrame;
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
                    int currentWaitms = _stellaServer.Animator.TransformationController.AnimationTransformation.MasterAnimationTransformationSettings.TimeUnitsPerFrame;
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

                    _stellaServer.Animator.TransformationController.SetTimeUnitsPerFrame(newWaitMs);
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
                    _stellaServer.Animator.TransformationController.SetBrightnessCorrection(brightnessCorrection);
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
                    _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(transformationFactor);
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
                    _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(transformationFactor);
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
                    _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(transformationFactor);
                }
               
            }
        }

        

        static bool ValidateCommandLineArguments(string mappingFilePath, string ip, int port,int udpPort, string storyboardDirPath, string apiIp, int apiPort, string bitmapDirectory,
            int millisecondsPerTimeUnit)
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

            if (millisecondsPerTimeUnit < 1)
            {
                Console.Out.WriteLine("The milliseconds per time unit must be larger than 0");
            }

            return true;
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

        private const string _HELP_TEXT = @"
StellaServer
[Usage]
    StellaServer -ip <ip> -port <port> -udp_port <udp port> -m <mapping file path> -s <storyboard directory path> -b <bitmap directory path>

[Required]
    -ip         Ip address of this machine. The pi's will connect to this address.
    -port       Tcp port the pi's will connect to.
    -udp_port   Udp port the pi's will connect to.
    -m          Path to mapping configuration file.
    -s          Path to the directory containig storyboards.
    -b          Path to the directory containig bitmaps, which will be converted to storyboards.

[Optional]
    -api_ip     Ip adress of this machine. The webserver will connect to this address.  Default = null.
    -api_port   Port of this machine. The webserver will connect to this port. Default = null.
    -t          The number of miliseconds each time unit takes. Default = 1ms.
    -h          Print this help page.
";
    }
}
