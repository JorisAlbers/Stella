using StellaClient.Network;
using StellaClient.Time;
using System;
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