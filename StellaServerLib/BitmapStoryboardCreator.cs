using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib
{
    /// <summary>
    ///  Creates storyboards from bitmap images
    /// </summary>
    public class BitmapStoryboardCreator
    {
        private readonly DirectoryInfo _bitmapDirectory;
        private readonly int _rowSize;
        private readonly int _numberOfPis;
        private readonly int _rowsPerPi;
        private int _totalNumberOfPixels;

        public BitmapStoryboardCreator(DirectoryInfo bitmapDirectory, int rowSize, int numberOfPis, int rowsPerPi)
        {
            _bitmapDirectory = bitmapDirectory;
            _rowSize = rowSize;
            _numberOfPis = numberOfPis;
            _rowsPerPi = rowsPerPi;

            _totalNumberOfPixels = rowSize * rowsPerPi * numberOfPis;
        }

        /// <summary>
        /// Appends the bitmap storyboards to the list of storyboards
        /// </summary>
        /// <param name="storyboards"></param>
        public void Create(List<Storyboard> storyboards)
        {
            // Iterate folders in directory
            foreach (DirectoryInfo subDirectory in _bitmapDirectory.GetDirectories())
            {
                foreach (FileInfo fileInfo in subDirectory.GetFiles())
                {
                    if (fileInfo.Extension == ".png")
                    {
                        AddBitmapAnimation(storyboards, Path.Combine(subDirectory.Name, Path.GetFileNameWithoutExtension(fileInfo.Name)));
                    }
                }
            }

            // Iterate files in bitmap dir
            foreach (FileInfo fileInfo in _bitmapDirectory.GetFiles())
            {
                if (fileInfo.Extension == ".png")
                {
                    AddBitmapAnimation(storyboards, Path.GetFileNameWithoutExtension(fileInfo.Name));
                }
            }
        }

        private void AddBitmapAnimation(List<Storyboard> storyboards, string name)
        {
            Storyboard sb = new Storyboard();
            sb.Name = name;
            
            if (name.Contains("Full_Setup"))
            {
                sb.Name = name;
                sb.AnimationSettings = new IAnimationSettings[]
                {
                        new BitmapAnimationSettings
                        {
                            FrameWaitMs = 10,
                            ImageName = name,
                            StripLength = _totalNumberOfPixels,
                            Wraps = true
                        }
                };
            }
            else
            {
                // Add animation per row
                int animations = _numberOfPis * _rowsPerPi;
                sb.AnimationSettings = new IAnimationSettings[animations];

                for (int i = 0; i < animations; i++)
                {
                    sb.AnimationSettings[i] = new BitmapAnimationSettings
                    {
                        FrameWaitMs = 10,
                        ImageName = name,
                        StartIndex = _rowSize * i,
                        StripLength = _rowSize,
                        Wraps = true
                    };
                }
            }
            storyboards.Add(sb);

        }
    }
}
