using System;
using System.IO;
using System.Threading;
using StellaClientLib.Network;
using StellaClientLib.Serialization;

namespace StellaClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                OutputHelp();
                return;
            }

            // Parse args
            string configurationFilepath = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i][0] == '-')
                {
                    // arg is a flag
                    switch (args[i])
                    {
                        case "-h":
                            OutputHelp();
                            return;
                        case "-c":
                            configurationFilepath = args[++i];
                            break;
                       default:
                            Console.Out.WriteLine($"Unknown flag {args[i]}");
                            return;
                    }
                }
            }

            if (!File.Exists(configurationFilepath))
            {
                Console.Out.WriteLine("The configuration file does not exits.");
                return;
            }

            // Get the configuration
            Configuration configuration;
            try
            {
                ConfigurationLoader configurationLoader = new ConfigurationLoader();
                using (StreamReader reader = new StreamReader(configurationFilepath))
                {
                    configuration = configurationLoader.Load(reader);
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to load the configuration.");
                OutputError(e);
                return;
            }

            // Start StellaClient
            StellaClientLib.StellaClient stellaClient;
            try
            {
                stellaClient = new StellaClientLib.StellaClient(configuration, new StellaServerFactory());
                stellaClient.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start StellaClient");
                OutputError(e);
                return;
            }

            // Wait for cancel signal
            var autoResetEvent = new AutoResetEvent(false);
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                // cancel the cancellation to allow the program to shutdown cleanly
                eventArgs.Cancel = true;
                autoResetEvent.Set();
            };

            // main blocks here waiting for ctrl-C
            autoResetEvent.WaitOne();
            Console.WriteLine("Manual shutdown.");
            stellaClient.Dispose();
        }
      
        static void OutputHelp()
        {
            Console.Out.WriteLine("Stella Client");
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("How to run: StellaClient -c <configuration_file>");
        }

        static void OutputError(Exception e)
        {
            Console.Out.WriteLine(e.Message);
            Console.Out.WriteLine(e.StackTrace);
            if (e.InnerException != null)
            {
                Console.Out.WriteLine(e.InnerException.Message);
                Console.Out.WriteLine(e.InnerException.StackTrace);
            }
        }
    }
}
