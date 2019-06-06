using System;
using StellaServerLib;

namespace StellaServerConsole
{
    class Program
    {
        private static StellaServer _stellaServer;


        static void Main(string[] args)
        {
            Console.WriteLine("Starting StellaServer");

            // Parse args
            string mappingFilePath = null;
            string ip = null;
            int port = 0;

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
                        case "-ip":
                            ip = args[++i];
                            break;
                        case "-port":
                            port = int.Parse(args[++i]);
                            break;
                        default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }
            ValidateCommandLineArguments(mappingFilePath,ip,port);
            

            _stellaServer = new StellaServer(mappingFilePath, ip, port);

            try
            {
                _stellaServer.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("An exception occured when starting StellaServer.");
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.StackTrace);

                if (e.InnerException != null)
                {
                    Console.Out.WriteLine("InnerException :");
                    Console.Out.WriteLine(e.InnerException.Message);
                    Console.Out.WriteLine(e.InnerException.StackTrace);
                }

            }

            Console.ReadLine();
        }

        static void ValidateCommandLineArguments(string mappingFilePath, string ip, int port)
        {
            if (mappingFilePath == null)
            {
                Console.Out.WriteLine("The mapping file must be set. Use -m <mapping_filepath>");
                Console.ReadLine();
                return;
            }

            if (port == 0)
            {
                Console.Out.WriteLine("The port must be set. Use -port <port value>");
                Console.ReadLine();
                return;
            }

            if (ip == null)
            {
                Console.Out.WriteLine("The ip must be set. Use -ip <ip value>");
                Console.ReadLine();
                return;
            }

        }


        static void outputHelp()
        {
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("To run: StellaServer -m <mapping_filepath>");
            Console.Out.WriteLine();
        }
    }
}
