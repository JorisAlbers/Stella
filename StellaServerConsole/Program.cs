﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

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
            string storyboardDirPath = null;
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
                            OutputHelp();
                            return;
                        case "-m":
                            mappingFilePath = args[++i];
                            break;
                        case "-s":
                            storyboardDirPath = args[++i];
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
            if(!ValidateCommandLineArguments(mappingFilePath,ip,port, storyboardDirPath))
            {
                return;
            }

            // Load animations from disc
            List<Storyboard> storyboards = LoadAnimations(storyboardDirPath);
            if (storyboards.Count < 1)
            {
                Console.Out.WriteLine("No storyboards found!");
                return;
            }
            string[] storyboardNames = storyboards.Select(x => x.Name).ToArray();
            

            _stellaServer = new StellaServer(mappingFilePath, ip, port);

            try
            {
                _stellaServer.Start();
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("An exception occured when starting StellaServer.");
                OutputError(e);
                return;
            }

            while (true)
            {
                OutputMenu(storyboardNames);
                string input = Console.ReadLine();
                if (input == "q")
                {
                    _stellaServer.Dispose();
                    break;
                }

                if (int.TryParse(input, out int storyboardToPlay))
                {
                    if (storyboardToPlay < 0 || storyboardToPlay > storyboards.Count -1)
                    {
                        Console.Out.WriteLine("Invalid animation");
                        continue;
                    }

                    try
                    {
                        _stellaServer.StartStoryboard(storyboards[storyboardToPlay]);
                    }
                    catch (Exception e)
                    {
                        Console.Out.WriteLine($"Failed to start storyboard.");
                        OutputError(e);
                    }
                }
            }

            Console.Out.WriteLine("End of StellaServer.");
        }

        static bool ValidateCommandLineArguments(string mappingFilePath, string ip, int port, string storyboardDirPath)
        {
            // TODO path and file exist validation
            if (mappingFilePath == null)
            {
                Console.Out.WriteLine("The mapping file must be set. Use -m <mapping_filepath>");
                return false;
            }

            if (port == 0)
            {
                Console.Out.WriteLine("The port must be set. Use -port <port value>");
                return false;
            }

            if (ip == null)
            {
                Console.Out.WriteLine("The ip must be set. Use -ip <ip value>");
                return false;
            }

            if (storyboardDirPath == null)
            {
                Console.Out.WriteLine("The storyboard directory path must be set. Use -s <dir path>");
                return false;
            }

            if (!Directory.Exists(storyboardDirPath))
            {
                Console.Out.WriteLine("The storyboard directory path does not point to an existing directory");
                return false;
            }

            return true;
        }

        static void OutputHelp()
        {
            Console.Out.WriteLine("StellaServer");
            Console.Out.WriteLine("To run: StellaServer -m <mapping_filepath>");
            Console.Out.WriteLine();
        }

        static void OutputMenu(string[] storyboardNames)
        {
            Console.Out.WriteLine("Exit program = q");
            Console.Out.WriteLine("Start animation:");
            for (int i = 0; i < storyboardNames.Length; i++)
            {
                Console.Out.WriteLine($"{i} - {storyboardNames[i]}");
            }
            Console.Out.WriteLine();
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

        private static List<Storyboard> LoadAnimations(string storyboardDirPath)
        {
            IEnumerable<FileInfo> files = new DirectoryInfo(storyboardDirPath).GetFiles().Where(x=>x.Extension == ".yaml");
            StoryboardLoader storyboardLoader = new StoryboardLoader();
            List<Storyboard> storyboards = new List<Storyboard>();
            foreach (FileInfo fileInfo in files)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(fileInfo.OpenRead()))
                    {
                        storyboards.Add(storyboardLoader.Load(reader));
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine($"Failed to load storyboard {fileInfo.Name}");
                    Console.Out.WriteLine(e.Message);
                }
            }

            return storyboards;
        }
    }
}
