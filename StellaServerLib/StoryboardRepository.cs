using System;
using System.Collections.Generic;
using System.IO;
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

        public Storyboard Load(string name)
        {
            StoryboardLoader loader = new StoryboardLoader();
            return loader.Load(OpenStream(name));
        }

        private StreamReader OpenStream(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            if (!File.Exists(path))
            {
                throw new Exception($"There is no storyboard present at path {fullName}");
            }

            return new StreamReader(name);
        }

        private string GetFullName(string name)
        {
            return $"{name}.yaml";
        }

    }
}
