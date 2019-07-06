﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace StellaServerLib
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
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            return File.Exists(GetFullName(path));
        }

        public void Save(Bitmap bitmap, string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            if (File.Exists(path))
            {
                throw new Exception($"There already is a bitmap present at path {fullName}");
            }

            bitmap.Save(path, ImageFormat.Png);
        }

        public Bitmap Load(string name)
        {
            string fullName = GetFullName(name);
            string path = Path.Combine(_directoryPath, fullName);

            if (!File.Exists(path))
            {
                throw new Exception($"There is no bitmap present at path {fullName}");
            }

            return new Bitmap(Image.FromFile(path));
        }

        private string GetFullName(string name)
        {
            return $"{name}.png";
        }
    }
}
