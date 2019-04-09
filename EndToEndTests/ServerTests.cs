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

            List<Frame> frames1 = repeatingPatternsAnimator.Create();

            AnimationExpander expander = new AnimationExpander(frames1);
            frames1 = expander.Expand(100);


            string input;
            Console.Out.WriteLine($"Started StellaServer instance on port {port}");
            Console.Out.WriteLine("a - Start new animation");

            while ((input = Console.ReadLine()) != "q")
            {
                Console.Out.WriteLine("a - Start new animation");
                Console.Out.WriteLine("q - quit");

                switch (input)
                {
                    case "a":
                        clientController.StartAnimation(new FrameSet(DateTime.Now + TimeSpan.FromSeconds(1))
                        {
                            Frames = frames1
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