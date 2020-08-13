using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BitmapScaler
{
    class Program
    {
        private const string BitmapInputDir = @"..\..\..\..\StellaServer\Resources\Bitmaps\Cloud";
        private const string BitmapOutputDir = @"\Bitmaps";
        private const int _MAX_ROW_SIZE = 480;
        private const int _MAX_TOTAL_SIZE = 2880;

        static void Main(string[] args)
        {
            if (!Directory.Exists(BitmapOutputDir))
            {
                Directory.CreateDirectory(BitmapOutputDir);
            }

            DicSearch(BitmapInputDir);
        }

        private static void DicSearch(string directory)
        {
            foreach (string enumerateFile in Directory.EnumerateFiles(directory))
            {
                Console.WriteLine($"scaling image {enumerateFile}");
                Bitmap original = (Bitmap)Image.FromFile(enumerateFile);

                Bitmap resized;
                string fileName = enumerateFile.Split("\\").Last();
                string nonResizedFileName = fileName.Replace(".png", "_originalSize.png");

                if (directory.Contains("\\F\\"))
                {
                    resized = new Bitmap(original, new Size(_MAX_TOTAL_SIZE, original.Height));
                }
                else
                {
                    resized = new Bitmap(original, new Size(_MAX_ROW_SIZE, original.Height));
                }

                string subFolderPath = directory.Split(BitmapOutputDir).Last();
                string dirpath = Path.Combine(BitmapOutputDir, subFolderPath);

                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }

                string path = Path.Combine(dirpath, fileName);
                string pathNonResized = Path.Combine(dirpath, nonResizedFileName);


                Console.Out.WriteLine($"Writing to path {path}");
                resized.Save(path);
                original.Save(pathNonResized);
            }

            foreach (string subDir in Directory.EnumerateDirectories(directory))
            {
                DicSearch(subDir);
            }
            
        }
    }
}
