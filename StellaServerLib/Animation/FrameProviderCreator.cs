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

        public FrameProviderCreator(BitmapRepository bitmapRepository, int millisecondsPerTimeUnit)
        {
            _bitmapRepository = bitmapRepository;
            _millisecondsPerTimeUnit = millisecondsPerTimeUnit;
        }

        public IFrameProvider Create(Storyboard storyboard, TransformationSettings masterTransformationSettings, out TransformationController transformationController)
        {
            TransformationSettings[] animationTransformationSettings = new TransformationSettings[storyboard.AnimationSettings.Length];
            transformationController = new TransformationController(masterTransformationSettings, animationTransformationSettings);

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
                    drawers[i] = new BitmapDrawer(bitmapAnimationSettings.StartIndex, bitmapAnimationSettings.StripLength, bitmapAnimationSettings.Wraps, bitmapToFramesDictionary[bitmapAnimationSettings.ImageName]);
                }
                else
                {
                    drawers[i] = CreateDrawer(settings);
                }

                animationTransformationSettings[i] = new TransformationSettings(settings.TimeUnitsPerFrame, 0, new float[3]);
                relativeTimeStamps[i] = settings.RelativeStart;
            }

            // Then, create a new FrameProvider
            return new FrameProvider(drawers, relativeTimeStamps, transformationController, _millisecondsPerTimeUnit);
        }

        private IDrawer CreateDrawer(IAnimationSettings animationSetting)
        {
            if (animationSetting is MovingPatternAnimationSettings movingPatternSetting)
            {
                return new MovingPatternDrawer(movingPatternSetting.StartIndex, movingPatternSetting.StripLength, movingPatternSetting.Pattern);
            }
            if (animationSetting is SlidingPatternAnimationSettings slidingPatternSetting)
            {
                return new SlidingPatternDrawer(slidingPatternSetting.StartIndex, slidingPatternSetting.StripLength, slidingPatternSetting.Pattern);
            }
            if (animationSetting is RepeatingPatternAnimationSettings repeatingPatternSetting)
            {
                return new RepeatingPatternsDrawer(repeatingPatternSetting.StartIndex, repeatingPatternSetting.StripLength, repeatingPatternSetting.Patterns);
            }
            if (animationSetting is RandomFadeAnimationSettings randomFadeSetting)
            {
                return new RandomFadeDrawer(randomFadeSetting.StartIndex, randomFadeSetting.StripLength, randomFadeSetting.Pattern, randomFadeSetting.FadeSteps);
            }
            if (animationSetting is FadingPulseAnimationSettings fadingPulseSetting)
            {
                return new FadingPulseDrawer(fadingPulseSetting.StartIndex, fadingPulseSetting.StripLength, fadingPulseSetting.Color, fadingPulseSetting.FadeSteps);
            }

            if (animationSetting is BitmapAnimationSettings bitmapAnimationSettings)
            {
                return new BitmapDrawer(bitmapAnimationSettings.StartIndex, bitmapAnimationSettings.StripLength, bitmapAnimationSettings.Wraps, BitmapDrawer.CreateFrames(_bitmapRepository.Load(bitmapAnimationSettings.ImageName)));

            }

            throw new ArgumentException($"Failed to load drawer. Unknown drawer {animationSetting.GetType()}");
        }
    }
}
