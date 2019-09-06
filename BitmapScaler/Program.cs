using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace BitmapScaler
{
    class Program
    {
        private const string BitmapInputDir = @"..\..\..\..\StellaServerConsole\Resources\Bitmaps";
        private const string BitmapOutputDir = @"\Bitmaps";
        private const int _MAX_ROW_SIZE = 360;
        private const int _MAX_TOTAL_SIZE = 2160;

        static void Main(string[] args)
        {
            if (!Directory.Exists(BitmapOutputDir))
            {
                Directory.CreateDirectory(BitmapOutputDir);
            }

            // Iterate main folder
            foreach (string enumerateFile in Directory.EnumerateFiles(BitmapInputDir))
            {
                Console.WriteLine($"scaling image {enumerateFile}");
                Bitmap original = (Bitmap) Image.FromFile(enumerateFile);

                string fileName = enumerateFile.Split("\\").Last();
                var resized = new Bitmap(original, new Size(_MAX_ROW_SIZE, original.Height));
                

                string path = Path.Combine(BitmapOutputDir, fileName);
                Console.Out.WriteLine($"Writing to path {path}");
                resized.Save(path);
            }

            // Iterate subfolders
            foreach (string enumerateDirectory in Directory.EnumerateDirectories(BitmapInputDir))
            {
                foreach (string enumerateFile in Directory.EnumerateFiles(enumerateDirectory))
                {
                    Console.WriteLine($"scaling image {enumerateFile}");
                    Bitmap original = (Bitmap)Image.FromFile(enumerateFile);

                    Bitmap resized;
                    string fileName = enumerateFile.Split("\\").Last();
                    if (enumerateDirectory.Contains("Full_Setup"))
                    {
                        resized = new Bitmap(original, new Size(_MAX_TOTAL_SIZE, original.Height));
                    }
                    else
                    {
                        resized = new Bitmap(original, new Size(_MAX_ROW_SIZE, original.Height));
                    }

                    string subFolderPath = enumerateDirectory.Split("\\").Last();
                    string dirpath = Path.Combine(BitmapOutputDir, subFolderPath);

                    if (!Directory.Exists(dirpath))
                    {
                        Directory.CreateDirectory(dirpath);
                    }

                    string path = Path.Combine(dirpath, fileName);


                    Console.Out.WriteLine($"Writing to path {path}");
                    resized.Save(path);
                }
            }
        }
    }
}
