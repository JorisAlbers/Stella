using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Transformation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Animation
{// Creates an FrameProvider
    public class FrameProviderCreator : IFrameProviderCreator
    {
        private readonly BitmapRepository _bitmapRepository;
        private readonly int _millisecondsPerTimeUnit;
        // number of rows in the matrix of led tubes
        private readonly int _rows;
        // number or led tubes in a row
        private readonly int _columns;
        // how many led lights are in a column
        private readonly int _ledsPerColumn;


        public FrameProviderCreator(BitmapRepository bitmapRepository, int millisecondsPerTimeUnit, int rows, int columns, int ledsPerColumn)
        {
            _bitmapRepository = bitmapRepository;
            _millisecondsPerTimeUnit = millisecondsPerTimeUnit;
            _rows = rows;
            _columns = columns;
            _ledsPerColumn = ledsPerColumn;
        }

        public IFrameProvider Create(Storyboard storyboard, out StoryboardTransformationController transformationController)
        {
            AnimationTransformationSettings[] animationTransformationSettings = new AnimationTransformationSettings[storyboard.AnimationSettings.Length];
            transformationController = new StoryboardTransformationController(animationTransformationSettings);

            // First, get the drawers
            IDrawer[] drawers = new IDrawer[storyboard.AnimationSettings.Length];
            int[] relativeTimeStamps = new int[storyboard.AnimationSettings.Length];

            // Dirty check
            Dictionary<string, List<PixelInstruction>[]> bitmapToFramesDictionary = new Dictionary<string, List<PixelInstruction>[]>();

            for (int i = 0; i < storyboard.AnimationSettings.Length; i++)
            {
                IAnimationSettings settings = storyboard.AnimationSettings[i];

                if (settings is BitmapAnimationSettings bitmapAnimationSettings)
                {
                    if (!bitmapToFramesDictionary.ContainsKey(bitmapAnimationSettings.ImageName))
                    {
                        bitmapToFramesDictionary[bitmapAnimationSettings.ImageName] =
                            BitmapDrawer.CreateFrames(_bitmapRepository.Load(bitmapAnimationSettings.ImageName));
                    }
                    // TODO merge function below with CreateDrawer method.
                    GetPosition(bitmapAnimationSettings, out int startIndex, out int length);
                    drawers[i] = new BitmapDrawer(startIndex, length, bitmapAnimationSettings.Wraps, bitmapToFramesDictionary[bitmapAnimationSettings.ImageName]);
                }
                else
                {
                    drawers[i] = CreateDrawer(settings);
                }

                animationTransformationSettings[i] = new AnimationTransformationSettings(0, 0, new float[3]{1,1,1}, false);
                relativeTimeStamps[i] = settings.RelativeStart;
            }

            // Then, create a new FrameProvider
            return new FrameProvider(drawers, relativeTimeStamps, transformationController, _millisecondsPerTimeUnit);
        }

        private IDrawer CreateDrawer(IAnimationSettings animationSetting)
        {
            GetPosition(animationSetting, out int startIndex, out int length);

            if (animationSetting is MovingPatternAnimationSettings movingPatternSetting)
            {
                return new MovingPatternDrawer(startIndex, length, movingPatternSetting.Pattern);
            }
            if (animationSetting is SlidingPatternAnimationSettings slidingPatternSetting)
            {
                return new SlidingPatternDrawer(startIndex, length, slidingPatternSetting.Pattern);
            }
            if (animationSetting is RepeatingPatternAnimationSettings repeatingPatternSetting)
            {
                return new RepeatingPatternsDrawer(startIndex, length, repeatingPatternSetting.Patterns);
            }
            if (animationSetting is RandomFadeAnimationSettings randomFadeSetting)
            {
                return new RandomFadeDrawer(startIndex, length, randomFadeSetting.Pattern, randomFadeSetting.FadeSteps);
            }
            if (animationSetting is FadingPulseAnimationSettings fadingPulseSetting)
            {
                return new FadingPulseDrawer(fadingPulseSetting.StartIndex, length, fadingPulseSetting.Color, fadingPulseSetting.FadeSteps);
            }

            if (animationSetting is BitmapAnimationSettings bitmapAnimationSettings)
            {
                return new BitmapDrawer(startIndex, length,
                    bitmapAnimationSettings.Wraps,
                    BitmapDrawer.CreateFrames(_bitmapRepository.Load(bitmapAnimationSettings.ImageName)));
            }

            throw new ArgumentException($"Failed to load drawer. Unknown drawer {animationSetting.GetType()}");
        }

        private void GetPosition(IAnimationSettings settings, out int startIndex, out int length)
        {
            startIndex = settings.StartIndex;
            length = settings.StripLength;

            if (settings.StretchToCanvas)
            {
                startIndex = 0;
                length = _rows * _columns * _ledsPerColumn;
            }
            else if (settings.RowIndex != -1)
            {
                startIndex = settings.RowIndex * _columns * _ledsPerColumn;
                length = _columns * _ledsPerColumn;
                // TODO implement settings animations per column
            }
        }
    }
}
