using System;
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
            Server server = new Server(20055);
            Console.WriteLine("Starting server instance, press enter to quit");
            server.Start();
            Console.ReadLine();
            server.Dispose();
        }
    }
}