using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace StellaServerConsole
{
    /// <summary>
    /// Manages storing of bitmaps
    /// </summary>
    public class BitmapRepository
    {
        private readonly string _directoryPath;

        public BitmapRepository(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException($"The directory at {directoryPath} does not exist.");
            }

            _directoryPath = directoryPath;
        }

        public bool BitmapExists(string name)
        {
            return File.Exists(GetFullName(name));
        }

        public void Save(Bitmap bitmap, string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            if (File.Exists(path))
            {
                throw new Exception($"There already is a bitmap present at path {fullName}");
            }

            bitmap.Save(fullName, ImageFormat.Png);
        }

        public Bitmap Load(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            if (!File.Exists(path))
            {
                throw new Exception($"There is no bitmap present at path {fullName}");
            }

            return new Bitmap(Image.FromFile(fullName));
        }

        private string GetFullName(string name)
        {
            return  $"{name}.png";
        }
    }
}
