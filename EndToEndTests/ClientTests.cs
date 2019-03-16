using rpi_ws281x;
using StellaClient.Light;
using StellaClient.Network;
using StellaClient.Time;
using StellaLib.Animation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

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
                Console.WriteLine("c - Run a StellaServer instance");
                Console.WriteLine("l - Ledcontroller tests");
                Console.WriteLine("t - Time related test");
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
                    case "l":
                        LedControllerTests();
                        break;                  
                    case "t":
                        TimeTests timeTests = new TimeTests();
                        timeTests.Start();
                        break;          
                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }
            }
        }

        private static void LedControllerTests()
        {
            int ledCount = 300;
            Settings settings = Settings.CreateDefaultSettings();
            settings.Channels[0] = new Channel(ledCount, 18, 255, false, StripType.WS2812_STRIP);
            WS281x ledstrip = new WS281x(settings);

            bool run = true;
            while(run)
            {
                Console.WriteLine("LedController tests");
                Console.WriteLine("m - Moving dot");
                Console.WriteLine("s - Color switch");
                Console.WriteLine("q - back");

                string input = Console.ReadLine();
                switch(input)
                {
                    case "q":
                        run = false;
                        break;
                    case "m":
                    {
                        int waitMS = 20;
                        Color color = Color.BlanchedAlmond;
                        Color clearColor = Color.FromArgb(0,0,0);

                        List<Frame> frames = new List<Frame>();
                        frames.Add(new Frame(0,waitMS){new PixelInstruction{ Index = 0, Color = color}});
                        for(int i=1; i < ledCount;i++)
                        {
                            frames.Add(new Frame(i,waitMS)
                            {
                                new PixelInstruction{ Index = (uint)i -1, Color = clearColor},
                                new PixelInstruction{ Index = (uint)i, Color = color}
                            });
                        }
                        frames.Add(new Frame(ledCount,waitMS){new PixelInstruction{ Index = (uint)ledCount-1, Color = clearColor}});

                        LedController controller = new LedController(ledstrip);
                        //LedController controller = new LedController(new FakeLedStrip());
                        controller.Run();
                        controller.AddFrames(frames);
                        Console.WriteLine("Press enter to quit");
                        Console.ReadLine();
                        controller.Dispose();
                    }
                        break;
                    case "s":
                    {
                        int waitMS = 100;

                        Color[] colors = new Color[]
                        {
                            Color.Red,
                            Color.Green,
                        };
                        List<Frame> frames = new List<Frame>();
                        for(int i = 0; i< 100; i++)
                        {
                            for(int j=0;j < colors.Length;j++)
                            {
                                Frame frame = new Frame(j,waitMS);
                                for(uint k = 0; k<ledCount;k++)
                                {
                                    frame.Add(new PixelInstruction{Index = k, Color = colors[j]});
                                }
                                frames.Add(frame);
                            }
                        }
                        LedController controller = new LedController(ledstrip);
                        //LedController controller = new LedController(new FakeLedStrip());
                        controller.Run();
                        controller.AddFrames(frames);
                        Console.WriteLine("Press enter to quit");
                        Console.ReadLine();
                        controller.Dispose();
                    }
                    break;

                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }
            }
        }

        public static void RunStellaServerInstance()
        {
            Console.WriteLine("Enter ip:");
            string ip = Console.ReadLine();

            // Establish the local endpoint for the socket.  
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);

            StellaClient.Network.StellaServer stellaServer = new StellaClient.Network.StellaServer(localEndPoint, "ID_1", new LinuxTimeSetter());
            Console.WriteLine("Starting Client, press enter to quit");
            stellaServer.Start();
            Console.ReadLine();
            stellaServer.Dispose();
        }
    }
}