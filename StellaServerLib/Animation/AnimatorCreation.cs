using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Animation
{// Creates an Animator
    public class AnimatorCreation
    {
        private readonly BitmapRepository _bitmapRepository;

        public AnimatorCreation(BitmapRepository bitmapRepository)
        {
            _bitmapRepository = bitmapRepository;
        }

        public Animator Create(Storyboard storyboard, int[] stripLengthPerPi, List<PiMaskItem> mask)
        {
            IFrameProvider frameProvider;
            AnimationTransformation[] animationTransformations = new AnimationTransformation[storyboard.AnimationSettings.Length];

            if (storyboard.AnimationSettings.Length == 1)
            {
                // Use a normal drawer
                IDrawer drawer = CreateDrawer(storyboard.AnimationSettings[0]);
                AnimationTransformation animationTransformation = new AnimationTransformation(storyboard.AnimationSettings[0].FrameWaitMs);
                frameProvider = new FrameProvider(drawer,animationTransformation);
                animationTransformations[0] = animationTransformation;
            }
            else
            {
                // Use a CombinedFrameProvider to combine multiple frameProviders
                // First, get the drawers
                IFrameProvider[] frameProviders = new IFrameProvider[storyboard.AnimationSettings.Length];
                int[] relativeTimeStamps = new int[storyboard.AnimationSettings.Length];

                // Dirty check
                Dictionary<string, List<PixelInstructionWithoutDelta>[]> bitmapToFramesDictionary = new Dictionary<string, List<PixelInstructionWithoutDelta>[]>();

                for (int i = 0; i < storyboard.AnimationSettings.Length; i++)
                {
                    IAnimationSettings settings = storyboard.AnimationSettings[i];
                    IDrawer drawer;
                    AnimationTransformation animationTransformation;


                    if (settings is BitmapAnimationSettings bitmapAnimationSettings)
                    {
                        if (!bitmapToFramesDictionary.ContainsKey(bitmapAnimationSettings.ImageName))
                        {
                            bitmapToFramesDictionary[bitmapAnimationSettings.ImageName] =
                                BitmapDrawer.CreateFrames(_bitmapRepository.Load(bitmapAnimationSettings.ImageName));
                        }
                        animationTransformation = new AnimationTransformation(bitmapAnimationSettings.FrameWaitMs);
                        drawer = new BitmapDrawer(bitmapAnimationSettings.StartIndex, bitmapAnimationSettings.StripLength, bitmapAnimationSettings.Wraps, bitmapToFramesDictionary[bitmapAnimationSettings.ImageName]);
                    }
                    else
                    {
                        animationTransformation = new AnimationTransformation(settings.FrameWaitMs);
                        drawer = CreateDrawer(settings);
                    }                    
                    frameProviders[i] = new FrameProvider(drawer, animationTransformation);
                    animationTransformations[i] = animationTransformation;
                    relativeTimeStamps[i] = settings.RelativeStart;
                }

                // Then, combine them in a single frame provider by using the CombinedFrameProvider
                frameProvider = new CombinedFrameProvider(frameProviders, relativeTimeStamps);
            }

            return new Animator(frameProvider, stripLengthPerPi, mask, animationTransformations);
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
