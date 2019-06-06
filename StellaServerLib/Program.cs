using System;

namespace StellaServer
{
    class Program
    {
        private static StellaServer _stellaServer;


        static void Main(string[] args)
        {
            Console.WriteLine("Starting StellaServer");

            // Parse args
            string mappingFilePath = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    // arg is a flag
                    switch (args[i])
                    {
                        case "-h":
                            outputHelp();
                            return;
                        case "-m":
                            mappingFilePath = args[++i];
                            break;
                        default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }

            if (mappingFilePath == null)
            {
                Console.Out.WriteLine("The mapping file must be set. Use -m <mapping_filepath>");
                return;
            }

            _stellaServer = new StellaServer(mappingFilePath);

            try
            {
                _stellaServer.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("An exception occured when starting StellaServer.");
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.StackTrace);
            }

            Console.ReadLine();
        }


        static void outputHelp()
        {
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("To run: StellaServer -m <mapping_filepath>");
            Console.Out.WriteLine();
        }
    }
}
