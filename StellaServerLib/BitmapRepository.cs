using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Abstractions;
using OpenCvSharp;

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
            Directory.CreateDirectory(directoryPath);

            _fileSystem = fileSystem;
            _directory = fileSystem.Directory.CreateDirectory(directoryPath);
        }

        public string FolderPath => _directory.FullName;

        public bool BitmapExists(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);

            return _fileSystem.File.Exists(path);
        }

        public void Save(Bitmap bitmap, string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);

            
            var fileInfo = new FileInfo(path);
            
            if (fileInfo.Exists)
            {
                throw new Exception($"There already is a bitmap present at path {fullName}");
            }
            
            fileInfo.Directory.Create();


            bitmap.Save(path, ImageFormat.Png);
        }

        public void Save(Mat bitmap, string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directory.FullName, fullName);


            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists)
            {
                throw new Exception($"There already is a bitmap present at path {fullName}");
            }

            fileInfo.Directory.Create();

            Cv2.ImWrite(path, bitmap);
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
            InternalListAllBitmaps(_directory, bitmapList);
            return bitmapList;
        }

        private void InternalListAllBitmaps(IDirectoryInfo directory, List<string> bitmapList)
        {
            // Iterate files in bitmap dir
            foreach (IFileInfo fileInfo in directory.GetFiles())
            {
                if (fileInfo.Extension == ".png")
                {
                    bitmapList.Add(fileInfo.FullName.Replace(_directory.FullName, "").Replace(".png",""));
                }
            }

            // Iterate folders in directory
            foreach (IDirectoryInfo subDirectory in directory.GetDirectories())
            {
                InternalListAllBitmaps(subDirectory, bitmapList);
            }
        }
    }
}
