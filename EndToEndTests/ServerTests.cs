using System;
using System.Collections.Generic;
using System.Drawing;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;
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

                string input = Console.ReadLine();
                switch(input)
                {
                    case "q":
                        run = false;
                        break;
                    case "s":
                        StartServerInstance();
                        break;
                    default:
                        Console.WriteLine($"Unknown command {input}");
                        break;
                }
            }
        }

        private static void StartServerInstance()
        {
            Server server = new Server();
            Console.WriteLine("Starting server instance, press enter to quit");
            server.Start(Program.SERVER_IP, 20055, 20056, 20056);
            Console.ReadLine();
            server.Dispose();
        }
    }
}