using System;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;
using StellaServer;
using StellaServer.Animation;
using StellaServer.Animation.Animators;
using StellaServer.Network;

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
            int port = 20055;
            Console.Out.WriteLine("Starting StellaServer instance");
            Server server = new Server("192.168.1.110",port);
            server.Start();
            ClientController clientController = new ClientController(server);


            RepeatingPatternsAnimator repeatingPatternsAnimator = new RepeatingPatternsAnimator(300, 100, new List<Color[]>
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
                    Color.FromArgb(0,0,0)
                }

            });
            AnimationExpander expander = new AnimationExpander(repeatingPatternsAnimator.Create());
            List<Frame> frames1 = expander.Expand(100);

            SlidingPatternAnimator slidingPatternAnimator = new SlidingPatternAnimator(300,100,new Color[]
            {
                Color.Green, Color.Red, Color.Blue, Color.Wheat, Color.Aqua, Color.Green, Color.Orange, Color.Orange, Color.FromArgb(0,0,0)
            });

            AnimationExpander expander2 = new AnimationExpander(slidingPatternAnimator.Create());
            List<Frame> frames2 =  expander2.Expand(110);

            MovingPatternAnimator movingPatternAnimator = new MovingPatternAnimator(300,30,new Color[]
            {
                Color.FromArgb(0,0,0),
                Color.FromArgb(0,0,25),
                Color.FromArgb(0,0,50),
                Color.FromArgb(0,0,75),
                Color.FromArgb(0,0,100),
                Color.FromArgb(0,0,125),
                Color.FromArgb(0,0,150),
                Color.FromArgb(0,0,175),
                Color.FromArgb(0,0,200),
                Color.FromArgb(0,0,225),
                Color.FromArgb(0,0,250),
            });

            List<Frame> movingDotFrames = movingPatternAnimator.Create();

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
                        clientController.StartAnimation(new FrameSet(DateTime.Now + TimeSpan.FromSeconds(1))
                        {
                            Frames = frames1
                        });
                        break;
                    case "s":
                        clientController.StartAnimation(new FrameSet(DateTime.Now + TimeSpan.FromSeconds(1))
                        {
                            Frames = frames2
                        });
                        break;
                    case "m":
                        clientController.StartAnimation(new FrameSet(DateTime.Now + TimeSpan.FromSeconds(1))
                        {
                            Frames = movingDotFrames
                        });
                        break;
                    default:
                        Console.Out.WriteLine("Unknown command.");
                        break;
                }

            }

        }

        private static void StartServerInstance()
        {
            Server server = new Server("192.168.1.110",20055);
            Console.WriteLine("Starting server instance, press enter to quit");
            server.Start();
            Console.ReadLine();
            server.Dispose();
        }
    }
}