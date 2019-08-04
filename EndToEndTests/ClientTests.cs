using rpi_ws281x;
using StellaLib.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using StellaClientLib.Light;
using StellaClientLib.Network;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;

namespace EndToEndTests
{
    public class ClientTests
    {
        public static void Start()
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine("Client tests");
                Console.WriteLine("i0 - Run a StellaClient instance with ID 0");
                Console.WriteLine("i1 - Run a StellaClient instance with ID 1");
                Console.WriteLine("c - Run a StellaServer class instance");
                Console.WriteLine("q - back");

                string input = Console.ReadLine();
                switch(input)
                {
                    case "q":
                        run = false;
                        break;
                    case "c":
                        RunStellaServerInstance();
                        break;
                        break;                  
                    case "i0":
                        CreateStellaClientInstance(0);
                        break;
                    case "i1":
                        CreateStellaClientInstance(1);
                        break;

                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }
            }
        }

        private static void CreateStellaClientInstance(int id)
        {
            // Light
            int ledCount = 300;
            Settings settings = Settings.CreateDefaultSettings();
            settings.Channels[0] = new Channel(ledCount, 18, 255, false, StripType.WS2812_STRIP);
            WS281x ledstrip = new WS281x(settings);
            LedController ledController = new LedController(ledstrip,50);

            // Server
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(Program.SERVER_IP), 20055);
            StellaClientLib.Network.StellaServer stellaServer = new StellaClientLib.Network.StellaServer();
            stellaServer.RenderFrameReceived += (sender, frame) => ledController.RenderFrame(frame);
            stellaServer.Start(localEndPoint, 20056, id);
            
            string input;
            Console.Out.WriteLine($"Running StellaClient instance with id {id}");
            while ((input = Console.ReadLine()) != "q")
            {
                Console.Out.WriteLine("q - quit");

                switch (input)
                {
                    default:
                        Console.Out.WriteLine("Unknown command.");
                        break;
                }
            }
            stellaServer.Dispose();

        }

        public static void RunStellaServerInstance()
        {
            Console.WriteLine("Enter ip:");
            string ip = Console.ReadLine();

            int id = 0;

            // Establish the local endpoint for the socket.  
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);

            StellaServer stellaServer = new StellaServer();
            Console.WriteLine("Starting Client, press enter to quit");
            stellaServer.Start(localEndPoint, 20056, 0);
            Console.ReadLine();
            stellaServer.Dispose();
        }
    }
}