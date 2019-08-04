using System;
using System.IO;
using System.Net;
using System.Threading;
using rpi_ws281x;
using StellaClientLib.Light;
using StellaClientLib.Network;
using StellaClientLib.Serialization;

namespace StellaClient
{
    class Program
    {
        private static Configuration _configuration;
        private static LedController _ledController;
        private static StellaServer _stellaServer;


        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                OutputHelp();
                return;
            }

            // Parse args
            string configurationFilepath = null;
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
                        case "-c":
                            configurationFilepath = args[++i];
                            break;
                       default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }

            if (!File.Exists(configurationFilepath))
            {
                Console.Out.WriteLine("The configuration file does not exits.");
                return;
            }

            // Get the configuration
            try
            {
                ConfigurationLoader configurationLoader = new ConfigurationLoader();
                using (StreamReader reader = new StreamReader(configurationFilepath))
                {
                    _configuration = configurationLoader.Load(reader);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to load the configuration.");
                OutputError(e);
                return;
            }

            // Start the ledController
            try
            {
                Settings settings = new Settings(800000, _configuration.DmaChannel);
                settings.Channels[0] = new Channel(_configuration.LedCount, _configuration.PwmPin, _configuration.Brightness, false,
                    StripType.WS2812_STRIP);
                _ledController = new LedController(new WS281x(settings), _configuration.MinimumFrameRate);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start the ledController (Are you running with sudo rights?)");
                OutputError(e);
                return;
            }

            // Start the StellaServer connection
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_configuration.Ip), _configuration.Port);
                _stellaServer = new StellaServer(localEndPoint, _configuration.UdpPort, _configuration.Id);
                _stellaServer.RenderFrameReceived += (sender, frame) => _ledController.RenderFrame(frame);
                
                _stellaServer.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start opening a connection with the server.");
                OutputError(e);
                return;
            }

            // Wait for cancel signal
            var autoResetEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                // cancel the cancellation to allow the program to shutdown cleanly
                eventArgs.Cancel = true;
                autoResetEvent.Set();
            };

            // main blocks here waiting for ctrl-C
            autoResetEvent.WaitOne();
            Console.WriteLine("Manual shutdown.");
            _stellaServer.Dispose();
            
        }
      
        static void OutputHelp()
        {
            Console.Out.WriteLine("Stella Client");
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("How to run: StellaClient -c <configuration_file>");
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
