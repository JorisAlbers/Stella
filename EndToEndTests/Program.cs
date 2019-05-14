using System;

namespace EndToEndTests
{
    class Program
    {
        /// <summary> IP of the server. </summary>
        public static readonly string SERVER_IP = "192.168.1.110";
        //public static readonly string SERVER_IP = "192.168.0.100";

        static void Main(string[] args)
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine("End To End Test");
                Console.WriteLine("c - Client related tests");
                Console.WriteLine("s - Server related tests");
                Console.WriteLine("q - exit");


                string input = Console.ReadLine();
                switch(input)
                {
                    case "q":
                        run = false;
                        break;
                    case "c":
                        ClientTests.Start();
                        break;
                    case "s":
                        ServerTests.Start();
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

            Console.WriteLine("END");
        }
    }
}
