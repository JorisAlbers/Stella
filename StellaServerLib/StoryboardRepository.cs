using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib
{
    public class StoryboardRepository
    {
        private readonly string _directoryPath;

        public StoryboardRepository(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException($"The directory at {directoryPath} does not exist.");
            }

            _directoryPath = directoryPath;
        }

        public bool TryGetStoryboard(string name, out Storyboard storyboard)
        {
            string path = Path.Combine(_directoryPath, $"{name}.yaml");
            if (File.Exists(path))
            {
                StoryboardSerializer storyboardSerializer = new StoryboardSerializer();
                try
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        storyboard = storyboardSerializer.Load(reader);
                        return true;
                    }
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine($"Failed to load storyboard {name}");
                    Console.Out.WriteLine(e.Message);
                }
            }

            storyboard = null;
            return false;
        }


        public List<Storyboard> LoadStoryboards()
        {
            IEnumerable<FileInfo> files = new DirectoryInfo(_directoryPath).GetFiles().Where(x => x.Extension == ".yaml");
            StoryboardSerializer storyboardSerializer = new StoryboardSerializer();
            List<Storyboard> storyboards = new List<Storyboard>();
            foreach (FileInfo fileInfo in files)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(fileInfo.OpenRead()))
                    {
                        storyboards.Add(storyboardSerializer.Load(reader));
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
