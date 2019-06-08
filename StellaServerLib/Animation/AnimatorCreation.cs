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
{
    // Creates an Animator
    public static class AnimatorCreation
    {

        public static Animator Create(Storyboard storyboard, List<PiMaskItem> mask, DateTime startAt)
        {
            IDrawer drawer = CreateDrawer(storyboard.AnimationSettings);
            return new Animator(drawer,mask, startAt);
        }

        private static IDrawer CreateDrawer(IAnimationSettings[] storyboardAnimationSettings)
        {
            if (storyboardAnimationSettings.Length == 1)
            {
                // Use a normal drawer
                return CreateDrawer(storyboardAnimationSettings[0]);
            }
            else
            {
                // Use a SectionDrawer to combine multiple drawers
                // First, get the drawers
                IDrawer[] drawers = storyboardAnimationSettings.Select(CreateDrawer).ToArray();

                // TODO implement relativeTimestamps in the animationsettings.
                // TODO for now, start them at the same time.
                int[] relativeTimeStamps = Enumerable.Repeat(0, drawers.Length).ToArray();

                // Then, combine them in a single drawer by using the SectionDrawer
                return new SectionDrawer(drawers, relativeTimeStamps);
            }
        }

        private static IDrawer CreateDrawer(IAnimationSettings animationSetting)
        {
            if (animationSetting is MovingPatternAnimationSettings movingPatternSetting)
            {
                return new MovingPatternDrawer(movingPatternSetting.StartIndex, movingPatternSetting.StripLength, movingPatternSetting.FrameWaitMs, movingPatternSetting.Pattern);
            }
            if (animationSetting is SlidingPatternAnimationSettings slidingPatternSetting)
            {
                return new SlidingPatternDrawer(slidingPatternSetting.StartIndex, slidingPatternSetting.StripLength, slidingPatternSetting.FrameWaitMs, slidingPatternSetting.Pattern);
            }
            if (animationSetting is RepeatingPatternAnimationSettings repeatingPatternSetting)
            {
                return new RepeatingPatternsDrawer(repeatingPatternSetting.StartIndex, repeatingPatternSetting.StripLength, repeatingPatternSetting.FrameWaitMs, repeatingPatternSetting.Patterns.ToList());
            }
            if (animationSetting is RandomFadeAnimationSettings randomFadeSetting)
            {
                return new RandomFadeDrawer(randomFadeSetting.StartIndex, randomFadeSetting.StripLength, randomFadeSetting.FrameWaitMs, randomFadeSetting.Pattern, randomFadeSetting.FadeSteps);
            }
            if (animationSetting is FadingPulseAnimationSettings fadingPulseSetting)
            {
                return new FadingPulseDrawer(fadingPulseSetting.StartIndex, fadingPulseSetting.StripLength, fadingPulseSetting.FrameWaitMs, fadingPulseSetting.Color, fadingPulseSetting.FadeSteps);
            }
            if (animationSetting is BitmapAnimationSettings bitmapSetting)
            {
                Bitmap bitmap = new Bitmap(Image.FromFile(bitmapSetting.ImagePath));
                return new BitmapDrawer(bitmapSetting.StartIndex, bitmapSetting.StripLength, bitmapSetting.FrameWaitMs, bitmap);
            }
            
            throw new ArgumentException($"Failed to load drawer. Unknown drawer {animationSetting.GetType()}");
        }
    }
}
