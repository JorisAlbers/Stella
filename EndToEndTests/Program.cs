using System;

namespace EndToEndTests
{
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine("End To End Test");
                Console.WriteLine("e - stop the program");
                Console.WriteLine("t - Time related tests");

                string input = Console.ReadLine();
                switch(input)
                {
                    case "e":
                        run = false;
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
