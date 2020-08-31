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
        private readonly int _rowSize;
        private readonly int _numberOfPis;
        private readonly int _rowsPerPi;
        private int _totalNumberOfPixels;
        private int _animationsCount;


        public BitmapStoryboardCreator(BitmapRepository bitmapRepository, int rowSize, int numberOfPis, int rowsPerPi)
        {
            _bitmapRepository = bitmapRepository;
            _rowSize = rowSize;
            _numberOfPis = numberOfPis;
            _rowsPerPi = rowsPerPi;

            _totalNumberOfPixels = rowSize * rowsPerPi * numberOfPis;
            _animationsCount = _numberOfPis * _rowsPerPi;
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

        public Storyboard Create(string name, string imageName, LayoutType layoutType)
        {
            // Start at the same time
            IAnimationSettings[] settings = null;
            switch (layoutType)
            {
                case LayoutType.Straight:
                    settings = StartAtTheSameTime(imageName);
                    break;
                case LayoutType.ArrowHead:
                    settings = StartAsArrowHead(imageName);
                    break;
                case LayoutType.InversedArrowHead:
                    throw new NotImplementedException();
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
                        TimeUnitsPerFrame = 10,
                        ImageName = name,
                        StripLength = _totalNumberOfPixels,
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
                AnimationSettings = StartAsArrowHead(name)
            });

        }

        private IAnimationSettings[] StartAtTheSameTime(string name)
        {
            IAnimationSettings[] settings  = new IAnimationSettings[_animationsCount]; 

            for (int i = 0; i < _animationsCount; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    TimeUnitsPerFrame = 10,
                    ImageName = name,
                    StartIndex = _rowSize * i,
                    StripLength = _rowSize,
                    Wraps = true
                };
            }

            return settings;
        }

        private IAnimationSettings[] StartAtTheSameTimeNoWrap(string name)
        {
            IAnimationSettings[] settings = new IAnimationSettings[_animationsCount];

            for (int i = 0; i < _animationsCount; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    TimeUnitsPerFrame = 10,
                    ImageName = name,
                    StartIndex = _rowSize * i,
                    StripLength = _rowSize,
                };
            }

            return settings;
        }

        private IAnimationSettings[] StartAsArrowHead(string name)
        {
            IAnimationSettings[] settings = new IAnimationSettings[_animationsCount];

            float midpoint = (_animationsCount -1) / 2f;
            int timeIncrement = 500; // ms

            for (int i = 0; i < _animationsCount; i++)
            {
                settings[i] = new BitmapAnimationSettings
                {
                    TimeUnitsPerFrame = 10,
                    ImageName = name,
                    StartIndex = _rowSize * i,
                    StripLength = _rowSize,
                    RelativeStart = (int) (Math.Abs(midpoint - (i)) * timeIncrement)
                    
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
        InversedArrowHead,
    }
}
