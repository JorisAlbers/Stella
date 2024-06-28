using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
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
        private readonly BitmapRepository _bitmapRepository;
        private readonly BitmapRepository _resizedBitmapRepository;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _ledsPerColumn;
        private int _totalNumberOfPixels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows">Line of led tubes</param>
        /// <param name="columns">number of led tubes on a row (line)</param>
        /// <param name="bitmapRepository">Repository that contain the original files</param>
        /// <param name="resizedBitmapRepository">Repository that contain the resized files. If a bitmap has not yet been resized, they will be created here.</param>
        public BitmapStoryboardCreator(BitmapRepository bitmapRepository, BitmapRepository resizedBitmapRepository, int rows, int columns, int ledsPerColumn)
        {
            _bitmapRepository = bitmapRepository;
            _resizedBitmapRepository = resizedBitmapRepository;
            _rows = rows;
            _columns = columns;
            _ledsPerColumn = ledsPerColumn;

            _totalNumberOfPixels = rows * columns * ledsPerColumn;
        }

        /// <summary>
        /// Appends the bitmap storyboards to the list of storyboards
        /// </summary>
        /// <param name="storyboards"></param>
        public List<Storyboard> Create()
        {
            List<Storyboard> storyboards = new List<Storyboard>();
            // Iterate folders in directory
            foreach (string name in _bitmapRepository.ListAllBitmaps())
            {
                ResizeBitmap(name,_bitmapRepository, _resizedBitmapRepository);
                AddBitmapAnimation(storyboards, name);
            }

            return storyboards;
        }

        private void ResizeBitmap(string name, BitmapRepository bitmapRepository, BitmapRepository resizedBitmapRepository)
        {
            if (resizedBitmapRepository.BitmapExists(name))
            {
                return;
            }

            Bitmap original = bitmapRepository.Load(name);
            Bitmap resized;
            if (name.Contains("\\F\\"))
            {
                resized = new Bitmap(original, new Size(_totalNumberOfPixels, original.Height));
            }
            else
            {
                resized = new Bitmap(original, new Size(_columns * _ledsPerColumn, original.Height));
            }

            resizedBitmapRepository.Save(resized, name);
        }

        public Storyboard Create(string name, string imageName, LayoutType layoutType, int delay)
        {
            // Start at the same time
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[_rows];
            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = imageName,
                    RowIndex = i,
                };
            }

            switch (layoutType)
            {
                case LayoutType.Straight:
                    settings = StartAtTheSameTime(settings);
                    break;
                case LayoutType.ArrowHead:
                    settings = StartAsArrowHead(settings, delay);
                    break;
                case LayoutType.Dash:
                    settings = StartAsDash(settings, delay);
                    break;
                default:
                    throw new ArgumentException(nameof(layoutType));
            }


            return new Storyboard()
            {
                Name = name,
                AnimationSettings = settings,
            };
        }

        private void AddBitmapAnimation(List<Storyboard> storyboards, string name)
        {
            // Special case for animation in the Full Setup folder.
            if (name.Contains("\\F\\"))
            {
                Storyboard sb = new Storyboard();
                sb.Name = name;
                sb.AnimationSettings = new IAnimationSettings[]
                {
                    new BitmapAnimationSettings
                    {
                        ImageName = name,
                        StretchToCanvas = true,
                        Wraps = true
                    }
                };
                sb.StartTimeCanBeAdjusted = false;

                storyboards.Add(sb);
                return;

            }
            
            
            // Start at the same time
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[_rows];

            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = name,
                    RowIndex = i,
                    Wraps = true,
                };
            }

            storyboards.Add(new Storyboard
            {
                Name = name,
                AnimationSettings = StartAtTheSameTime(settings),
                StartTimeCanBeAdjusted = true,
            });
        }

        public static BitmapAnimationSettings[] StartAtTheSameTime(BitmapAnimationSettings[] animationSettings)
        {
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[animationSettings.Length];

            for (int i = 0; i < animationSettings.Length; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = animationSettings[i].ImageName,
                    RowIndex = animationSettings[i].RowIndex,
                    Wraps = true,
                };
            }
            return animationSettings;
        }

        public static BitmapAnimationSettings[] StartAtTheSameTimeNoWrap(BitmapAnimationSettings[] animationSettings)
        {
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[animationSettings.Length];

            for (int i = 0; i < animationSettings.Length; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = animationSettings[i].ImageName,
                    RowIndex = animationSettings[i].RowIndex,
                    Wraps = false,
                };
            }
            return animationSettings;
        }

        public static BitmapAnimationSettings[] StartAsArrowHead(BitmapAnimationSettings[] animationSettings, int delay)
        {
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[animationSettings.Length];
            float midpoint = (animationSettings.Length - 1) / 2f;

            for (int i = 0; i < animationSettings.Length; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = animationSettings[i].ImageName,
                    RowIndex = animationSettings[i].RowIndex,
                    RelativeStart = (int)(Math.Abs(midpoint - (i)) * delay)
                };
            }

            return settings;
        }

        public static BitmapAnimationSettings[] StartAsDash(BitmapAnimationSettings[] animationSettings, int delay)
        {
            BitmapAnimationSettings[] settings = new BitmapAnimationSettings[animationSettings.Length];
            
            for (int i = 0; i < animationSettings.Length; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = animationSettings[i].ImageName,
                    RowIndex = i,
                    RelativeStart = i  * delay
                };
            }

            return settings;
        }
    }

    public enum LayoutType
    {
        Unknown,
        Straight,
        ArrowHead,
        Dash,
    }
}
