using System;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Network;

namespace EndToEndTests
{
    public class ServerTests
    {
        public static void Start()
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine("Server tests");
                Console.WriteLine("q - back");
                Console.WriteLine("s - start server instance");
                Console.Out.WriteLine("i - Start StellaServer instance");

                string input = Console.ReadLine();
                switch(input)
                {
                    case "q":
                        run = false;
                        break;
                    case "s":
                        StartServerInstance();
                        break;
                    case "i":
                        StartStellaServerInstance();
                        break;
                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }
            }
        }

        private static void StartStellaServerInstance()
        {
            int numberOfPis = 2;
            int lengthPerPi = 1200;
            int port = 20055;
            int udpPort = 20056;
            Console.Out.WriteLine("Starting StellaServer instance");
            Server server = new Server(Program.SERVER_IP,port, udpPort);
            server.Start();
            ClientController clientController = new ClientController(server);

            // Set mapping and create mask
            List<PiMapping> piMappings = new List<PiMapping>();
            for (int i = 0; i < numberOfPis; i++)
            {
                piMappings.Add(new PiMapping(i,lengthPerPi,0,new int[]{lengthPerPi/2},false));
            }
            PiMaskCalculator piMaskCalculator = new PiMaskCalculator(piMappings);
            List<PiMaskItem> mask = piMaskCalculator.Calculate(out int[] stripLengthPerPi);

            Color[] pattern = new Color[]
            {
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(0, 0, 25),
                Color.FromArgb(0, 0, 50),
                Color.FromArgb(0, 0, 75),
                Color.FromArgb(0, 0, 100),
                Color.FromArgb(0, 0, 125),
                Color.FromArgb(0, 0, 150),
                Color.FromArgb(0, 0, 175),
                Color.FromArgb(0, 0, 200),
                Color.FromArgb(0, 0, 225),
                Color.FromArgb(0, 0, 250),
            };
            IFrameProvider repeatingPatternsFrameProvider = new FrameProvider(
                new RepeatingPatternsDrawer(0, 300, new AnimationTransformation(100), new Color[][]
                {
                    new Color[2]
                    {
                        Color.Blue, Color.DarkBlue
                    },
                    new Color[3]
                    {
                        Color.DarkBlue, Color.DarkBlue, Color.Red
                    },
                    new Color[1]
                    {
                        Color.FromArgb(0, 0, 0)
                    }

                }), new AnimationTransformation(100));
            IFrameProvider slidingPatternFrameProvider = new FrameProvider(new SlidingPatternDrawer(0,300,new AnimationTransformation(100), pattern), new AnimationTransformation(100));
            IFrameProvider movingPatternFrameProvider = new FrameProvider(new MovingPatternDrawer(0,300, new AnimationTransformation(30), pattern), new AnimationTransformation(30));
            AnimationTransformation[] animationTransformations = new AnimationTransformation[]{new AnimationTransformation(0), };

            string input;
            Console.Out.WriteLine($"Started StellaServer instance on port {port}");
            Console.Out.WriteLine("a - Start new animation");

            while ((input = Console.ReadLine()) != "q")
            {
                Console.Out.WriteLine("r - Start RepeatingPattern animation");
                Console.Out.WriteLine("s - Start SlidingPattern   animation");
                Console.Out.WriteLine("m - Start MovingPattern    animation");
                Console.Out.WriteLine("q - quit");


                switch (input)
                {
                    case "r":
                        clientController.StartAnimation(new Animator(repeatingPatternsFrameProvider, stripLengthPerPi, mask, animationTransformations), DateTime.Now);
                        break;
                    case "s":
                        clientController.StartAnimation(new Animator(slidingPatternFrameProvider, stripLengthPerPi, mask, animationTransformations), DateTime.Now);
                        break;
                    case "m":
                        clientController.StartAnimation(new Animator(movingPatternFrameProvider, stripLengthPerPi, mask, animationTransformations), DateTime.Now);
                        break;
                    default:
                        Console.Out.WriteLine("Unknown command.");
                        break;
                }

            }

        }

        private static void StartServerInstance()
        {
            Server server = new Server(Program.SERVER_IP,20055, 20056);
            Console.WriteLine("Starting server instance, press enter to quit");
            server.Start();
            Console.ReadLine();
            server.Dispose();
        }
    }
}