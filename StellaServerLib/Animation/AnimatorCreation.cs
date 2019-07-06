﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
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
            IDrawer drawer;
            AnimationTransformation[] animationTransformations = new AnimationTransformation[storyboard.AnimationSettings.Length];

            if (storyboard.AnimationSettings.Length == 1)
            {
                // Use a normal drawer
                drawer = CreateDrawer(storyboard.AnimationSettings[0], out AnimationTransformation animationTransformation);
                animationTransformations[0] = animationTransformation;
            }
            else
            {
                // Use a SectionDrawer to combine multiple drawers
                // First, get the drawers
                IDrawer[] drawers = new IDrawer[storyboard.AnimationSettings.Length];
                int[] relativeTimeStamps = new int[storyboard.AnimationSettings.Length];

                for (int i = 0; i < storyboard.AnimationSettings.Length; i++)
                {
                    IAnimationSettings settings = storyboard.AnimationSettings[i];

                    drawers[i] = CreateDrawer(settings, out AnimationTransformation animationTransformation);
                    animationTransformations[i] = animationTransformation;
                    relativeTimeStamps[i] = settings.RelativeStart;
                }

                // Then, combine them in a single drawer by using the SectionDrawer
                drawer = new SectionDrawer(drawers, relativeTimeStamps);
            }

            return new Animator(drawer, stripLengthPerPi, mask, animationTransformations);
        }

        private IDrawer CreateDrawer(IAnimationSettings animationSetting, out AnimationTransformation animationTransformation)
        {
            animationTransformation = new AnimationTransformation(animationSetting.FrameWaitMs);

            if (animationSetting is MovingPatternAnimationSettings movingPatternSetting)
            {
                return new MovingPatternDrawer(movingPatternSetting.StartIndex, movingPatternSetting.StripLength, animationTransformation, movingPatternSetting.Pattern);
            }
            if (animationSetting is SlidingPatternAnimationSettings slidingPatternSetting)
            {
                return new SlidingPatternDrawer(slidingPatternSetting.StartIndex, slidingPatternSetting.StripLength, animationTransformation, slidingPatternSetting.Pattern);
            }
            if (animationSetting is RepeatingPatternAnimationSettings repeatingPatternSetting)
            {
                return new RepeatingPatternsDrawer(repeatingPatternSetting.StartIndex, repeatingPatternSetting.StripLength, animationTransformation, repeatingPatternSetting.Patterns);
            }
            if (animationSetting is RandomFadeAnimationSettings randomFadeSetting)
            {
                return new RandomFadeDrawer(randomFadeSetting.StartIndex, randomFadeSetting.StripLength, animationTransformation, randomFadeSetting.Pattern, randomFadeSetting.FadeSteps);
            }
            if (animationSetting is FadingPulseAnimationSettings fadingPulseSetting)
            {
                return new FadingPulseDrawer(fadingPulseSetting.StartIndex, fadingPulseSetting.StripLength, animationTransformation, fadingPulseSetting.Color, fadingPulseSetting.FadeSteps);
            }
            if (animationSetting is BitmapAnimationSettings bitmapSetting)
            {
                return new BitmapDrawer(bitmapSetting.StartIndex, bitmapSetting.StripLength, animationTransformation, bitmapSetting.Wraps, _bitmapRepository.Load(bitmapSetting.ImageName));
            }

            throw new ArgumentException($"Failed to load drawer. Unknown drawer {animationSetting.GetType()}");
        }
    }


}
