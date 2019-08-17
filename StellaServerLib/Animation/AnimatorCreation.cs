using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Animation
{// Creates an Animator
    public class AnimatorCreation
    {
        private readonly BitmapRepository _bitmapRepository;
        private TransformationController _transformationController;

        public AnimatorCreation(BitmapRepository bitmapRepository)
        {
            _bitmapRepository = bitmapRepository;
        }

        public Animator Create(Storyboard storyboard, int[] stripLengthPerPi, List<PiMaskItem> mask)
        {
            IFrameProvider frameProvider;
            TransformationSettings masterTransformationSettings;
            TransformationSettings[] animationTransformationSettings = new TransformationSettings[storyboard.AnimationSettings.Length];

            if (_transformationController != null)
            {
                masterTransformationSettings =_transformationController.AnimationTransformation.MasterTransformationSettings;
            }
            else
            {
                masterTransformationSettings = new TransformationSettings(0,0,new float[3]);
            }

            _transformationController = new TransformationController(masterTransformationSettings,animationTransformationSettings);


            if (storyboard.AnimationSettings.Length == 1)// TODO remove this redundant if statement
            {
                // Use a normal drawer 
                IDrawer drawer = CreateDrawer(storyboard.AnimationSettings[0]);
                animationTransformationSettings[0] = new TransformationSettings(storyboard.AnimationSettings[0].FrameWaitMs,0,new float[3]);

                frameProvider = new FrameProvider(drawer, _transformationController);
            }
            else
            {
                // First, get the drawers
                IDrawer[] drawers = new IDrawer[storyboard.AnimationSettings.Length];
                int[] relativeTimeStamps = new int[storyboard.AnimationSettings.Length];

                // Dirty check
                Dictionary<string, List<PixelInstructionWithoutDelta>[]> bitmapToFramesDictionary = new Dictionary<string, List<PixelInstructionWithoutDelta>[]>();

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

                    animationTransformationSettings[i] = new TransformationSettings(settings.FrameWaitMs, 0, new float[3]);
                    relativeTimeStamps[i] = settings.RelativeStart;
                }

                // Then, create a new FrameProvider with multiple animations
                frameProvider = new FrameProvider(drawers, relativeTimeStamps, _transformationController);
            }

            return new Animator(frameProvider, stripLengthPerPi, mask, _transformationController);
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
