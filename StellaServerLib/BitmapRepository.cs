using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Abstractions;

namespace StellaServerLib
{
    /// <summary>
    /// Manages storing of bitmaps
    /// </summary>
    public class BitmapRepository
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDirectoryInfo _directory;

        public BitmapRepository(IFileSystem fileSystem, string directoryPath)
        {
            if (!fileSystem.Directory.Exists(directoryPath))
            {
                throw new ArgumentException($"The directory at {directoryPath} does not exist.");
            }

            _fileSystem = fileSystem;
            _directory = fileSystem.Directory.CreateDirectory(directoryPath);
        }

        public bool BitmapExists(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);

            return _fileSystem.File.Exists(GetFullName(path));
        }

        public void Save(Bitmap bitmap, string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);

            if (File.Exists(path))
            {
                throw new Exception($"There already is a bitmap present at path {fullName}");
            }

            bitmap.Save(path, ImageFormat.Png);
        }

        public Bitmap Load(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);

            if (!_fileSystem.File.Exists(path))
            {
                throw new Exception($"There is no bitmap present at path {fullName}");
            }

            return new Bitmap(Image.FromFile(path));
        }

        private string GetFullName(string name)
        {
            return $"{name}.png";
        }

        public List<string> ListAllBitmaps()
        {
            List<string> bitmapList = new List<string>();
            
            // Iterate folders in directory
            foreach (IDirectoryInfo subDirectory in _directory.GetDirectories())
            {
                foreach (IFileInfo fileInfo in subDirectory.GetFiles())
                {
                    if (fileInfo.Extension == ".png")
                    {
                        bitmapList.Add(Path.Combine(subDirectory.Name, Path.GetFileNameWithoutExtension(fileInfo.Name)));
                    }
                }
            }

            // Iterate files in bitmap dir
            foreach (IFileInfo fileInfo in _directory.GetFiles())
            {
                if (fileInfo.Extension == ".png")
                {
                    bitmapList.Add(Path.GetFileNameWithoutExtension(fileInfo.Name));
                }
            }

            return bitmapList;
        }
    }
}
