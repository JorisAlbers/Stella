using rpi_ws281x;
using StellaClient.Light;
using StellaClient.Network;
using StellaClient.Time;
using StellaLib.Animation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Net;
using System.Threading;

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
                Console.WriteLine("slow - Slow Color switch");
                Console.WriteLine("p - Pending FramSet test");
                Console.WriteLine("clear - clear the ledstrip");
                Console.WriteLine("q - back");

                string input = Console.ReadLine();
                switch (input)
                {
                    case "q":
                        run = false;
                        break;
                    case "m":
                    {
                        int waitMS = 20;
                        Color color = Color.BlanchedAlmond;
                        Color clearColor = Color.FromArgb(0, 0, 0);

                        List<Frame> frames = new List<Frame>();
                        frames.Add(new Frame(0, 0) {new PixelInstruction {Index = 0, Color = color}});
                        for (int i = 1; i < ledCount; i++)
                        {
                            frames.Add(new Frame(i, i * waitMS)
                            {
                                new PixelInstruction {Index = (uint) i - 1, Color = clearColor},
                                new PixelInstruction {Index = (uint) i, Color = color}
                            });
                        }

                        frames.Add(new Frame(ledCount, ledCount * waitMS)
                            {new PixelInstruction {Index = (uint) ledCount - 1, Color = clearColor}});

                        LedController controller = new LedController(ledstrip);
                        //LedController controller = new LedController(new FakeLedStrip());
                        controller.Run();
                        controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
                        controller.AddFrames(frames);
                        Console.WriteLine("Press enter to quit");
                        Console.ReadLine();
                        controller.Dispose();
                    }
                        break;
                    case "s":
                    {
                        FrameSetMetadata metadata = new FrameSetMetadata(DateTime.Now);
                        int waitMS = 100;

                        Color[] colors = new Color[]
                        {
                            Color.Red,
                            Color.Green,
                        };
                        List<Frame> frames = new List<Frame>();
                        for (int i = 0; i < 100; i++)
                        {
                            for (int j = 0; j < colors.Length; j++)
                            {
                                Frame frame = new Frame(j, i * waitMS);
                                for (uint k = 0; k < ledCount; k++)
                                {
                                    frame.Add(new PixelInstruction {Index = k, Color = colors[j]});
                                }

                                frames.Add(frame);
                            }
                        }

                        LedController controller = new LedController(ledstrip);

                        controller.Run();
                        controller.PrepareNextFrameSet(metadata);
                        controller.AddFrames(frames);
                        Console.WriteLine("Press enter to quit");
                        Console.ReadLine();
                        controller.Dispose();
                    }
                        break;
                    case "p":
                    {
                        int waitMS = 1000;
                        //Animation 1
                        Color[] colors1 = new Color[]
                        {
                            Color.Red,
                            Color.Green,
                        };
                        int timeStampRelative = 0;
                        List<Frame> frames1 = new List<Frame>();
                        for (int i = 0; i < 100; i++)
                        {
                            for (int j = 0; j < colors1.Length; j++)
                            {
                                Frame frame = new Frame(j, timeStampRelative);
                                for (uint k = 0; k < ledCount; k++)
                                {
                                    frame.Add(new PixelInstruction {Index = k, Color = colors1[j]});
                                }

                                frames1.Add(frame);
                                timeStampRelative += waitMS;
                            }
                        }

                        // Animation 2
                        Color[] colors2 = new Color[]
                        {
                            Color.Blue,
                            Color.White,
                        };
                        List<Frame> frames2 = new List<Frame>();
                        timeStampRelative = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            for (int j = 0; j < colors2.Length; j++)
                            {
                                Frame frame = new Frame(j, timeStampRelative);
                                for (uint k = 0; k < ledCount; k++)
                                {
                                    frame.Add(new PixelInstruction {Index = k, Color = colors2[j]});
                                }

                                frames2.Add(frame);
                                timeStampRelative += waitMS;
                            }
                        }

                        LedController controller = new LedController(ledstrip);

                        controller.Run();
                        DateTime start = DateTime.Now;
                        controller.PrepareNextFrameSet(new FrameSetMetadata(start));
                        controller.AddFrames(frames1);

                        string userInput;
                        bool animationToggle = false;
                        Console.WriteLine("Press enter to prepare with next animation. q to quit ");
                        while ((userInput = Console.ReadLine()) != "q")
                        {

                            controller.PrepareNextFrameSet(
                                new FrameSetMetadata(DateTime.Now + TimeSpan.FromSeconds(1)));

                            if (animationToggle = !animationToggle)
                            {
                                controller.AddFrames(frames1);
                            }
                            else
                            {
                                controller.AddFrames(frames2);
                            }
                        }

                        controller.Dispose();
                    }
                        break;
                    case "slow":
                    {
                        int waitMS = 5000;
                        //Animation 1
                        Color[] colors1 = new Color[]
                        {
                            Color.LightGoldenrodYellow,
                            Color.LightGreen,
                        };
                        int timeStampRelative = 0;
                        List<Frame> frames1 = new List<Frame>();
                        int index = 0;
                        for (int i = 0; i < 100; i++)
                        {
                            for (int j = 0; j < colors1.Length; j++)
                            {
                                Frame frame = new Frame(index++, timeStampRelative);
                                for (uint k = 0; k < ledCount; k++)
                                {
                                    frame.Add(new PixelInstruction {Index = k, Color = colors1[j]});
                                }

                                frames1.Add(frame);
                                timeStampRelative += waitMS;
                            }
                        }

                        for (int i = 0; i < 10; i++)
                        {
                            Console.Out.WriteLine($"TSR: {frames1[i].TimeStampRelative} Color: {frames1[i][0].Color}");
                        }

                        LedController controller = new LedController(ledstrip);

                        controller.Run();
                        controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
                        controller.AddFrames(frames1);

                        string userInput;
                        bool animationToggle = false;
                        Console.WriteLine("Press enter to prepare with next animation. q to quit ");
                        while ((userInput = Console.ReadLine()) != "q")
                        {
                            controller.PrepareNextFrameSet(
                                new FrameSetMetadata(DateTime.Now + TimeSpan.FromSeconds(1)));

                            if (animationToggle = !animationToggle)
                            {
                                controller.AddFrames(frames1);
                            }
                        }

                        controller.Dispose();
                    }
                        break;
                    case "clear":
                    {
                        int waitMS = 0;
                        //Animation 1
                        Color[] colors1 = new Color[]
                        {
                            Color.FromArgb(0,0,0,0),
                        };
                        int timeStampRelative = 0;
                        List<Frame> frames1 = new List<Frame>();
                        Frame frame = new Frame(0, timeStampRelative);
                        for (uint k = 0; k < ledCount; k++)
                        {
                            frame.Add(new PixelInstruction { Index = k, Color = colors1[0] });
                        }
                        frames1.Add(frame);
                        LedController controller = new LedController(ledstrip);

                        controller.Run();
                        controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
                        controller.AddFrames(frames1);

                        Thread.Sleep(100);

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