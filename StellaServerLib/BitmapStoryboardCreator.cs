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
        private readonly BitmapRepository _bitmapRepository;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _ledsPerColumn;
        private int _totalNumberOfPixels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rows">Line of led tubes</param>
        /// <param name="columns">number of led tubes on a row (line)</param>
        public BitmapStoryboardCreator(BitmapRepository bitmapRepository, int rows, int columns, int ledsPerColumn)
        {
            _bitmapRepository = bitmapRepository;
            _rows = rows;
            _columns = columns;
            _ledsPerColumn = ledsPerColumn;

            _totalNumberOfPixels = rows * columns * ledsPerColumn;
            _rows = rows;
        }

        /// <summary>
        /// Appends the bitmap storyboards to the list of storyboards
        /// </summary>
        /// <param name="storyboards"></param>
        public List<Storyboard> Create()
        {
            List<Storyboard> storyboards = new List<Storyboard>();
            // Iterate folders in directory
            foreach (string bitmap in _bitmapRepository.ListAllBitmaps())
            {
                AddBitmapAnimation(storyboards, bitmap);
            }

            return storyboards;
        }

        public Storyboard Create(string name, string imageName, LayoutType layoutType, int delay)
        {
            // Start at the same time
            IAnimationSettings[] settings = null;
            switch (layoutType)
            {
                case LayoutType.Straight:
                    settings = StartAtTheSameTime(imageName);
                    break;
                case LayoutType.ArrowHead:
                    settings = StartAsArrowHead(imageName, delay);
                    break;
                case LayoutType.Dash:
                    settings = StartAsDash(imageName, delay);
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

                storyboards.Add(sb);
                return;

            }

            // Start at the same time
            storyboards.Add(new Storyboard
            {
                Name = name,
                AnimationSettings = StartAtTheSameTime(name)
            });

            // Start at the same time no wrap
            storyboards.Add(new Storyboard
            {
                Name = $"{name}_noWrap",
                AnimationSettings = StartAtTheSameTimeNoWrap(name)
            });

            // Arrow head
            storyboards.Add(new Storyboard
            {
                Name = $"{name}_arrowHead",
                AnimationSettings = StartAsArrowHead(name, 500)
            });

        }

        private IAnimationSettings[] StartAtTheSameTime(string name)
        {
            IAnimationSettings[] settings  = new IAnimationSettings[_rows]; 

            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = name,
                    RowIndex = i,
                    Wraps = true
                };
            }

            return settings;
        }

        private IAnimationSettings[] StartAtTheSameTimeNoWrap(string name)
        {
            IAnimationSettings[] settings = new IAnimationSettings[_rows];

            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = name,
                    RowIndex = i,
                };
            }

            return settings;
        }

        private IAnimationSettings[] StartAsArrowHead(string name, int delay)
        {
            IAnimationSettings[] settings = new IAnimationSettings[_rows];

            float midpoint = (_rows -1) / 2f;

            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = name,
                    RowIndex = i,
                    RelativeStart = (int) (Math.Abs(midpoint - (i)) * delay)
                    
                };
            }

            return settings;
        }

        private IAnimationSettings[] StartAsDash(string imageName, int delay)
        {
            IAnimationSettings[] settings = new IAnimationSettings[_rows];

            for (int i = 0; i < _rows; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    ImageName = imageName,
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
