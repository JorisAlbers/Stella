﻿using System.Drawing;
using NUnit.Framework;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Transformation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Test.Animation
{
    public class TestAnimatorCreator
    {

        [Test]
        public void MasterTransformationSettingsIsPreserved()
        {
            AnimatorCreator creator = new AnimatorCreator(new FrameProviderCreator(null,1), new int[] { 100 }, null);

            MovingPatternAnimationSettings animationSettings = new MovingPatternAnimationSettings
            {
                Pattern = new Color[] {Color.Red},
                RelativeStart = 10,
                StartIndex = 0,
                StripLength = 100
            };

            PlayList playList1 = new PlayList("playList1", new PlayListItem[] {new PlayListItem(new Storyboard{AnimationSettings = new IAnimationSettings[] { animationSettings } }, 0) });
            PlayList playList2 = new PlayList("playList2", new PlayListItem[] {new PlayListItem(new Storyboard{AnimationSettings = new IAnimationSettings[] { animationSettings } }, 0) });

            IAnimator animator = creator.Create(playList1);
            AnimationTransformationSettings expectedSettings =
                animator.StoryboardTransformationController.Settings.MasterSettings;

            animator = creator.Create(playList2);
            Assert.IsTrue(ReferenceEquals(expectedSettings, animator.StoryboardTransformationController.Settings.MasterSettings));
            
        }
    }
}
