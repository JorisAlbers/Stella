using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NUnit.Framework;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Transformation;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Test.Animation
{
    public class TestAnimatorFactory
    {

        [Test]
        public void MasterTransformationSettingsIsPreserved()
        {
            AnimatorFactory creator = new AnimatorFactory(null);

            MovingPatternAnimationSettings animationSettings = new MovingPatternAnimationSettings
            {
                FrameWaitMs = 10,
                Pattern = new Color[] {Color.Red},
                RelativeStart = 10,
                StartIndex = 0,
                StripLength = 100
            };


            Storyboard sb1 = new Storyboard {AnimationSettings = new IAnimationSettings[] {animationSettings}};
            Storyboard sb2 = new Storyboard {AnimationSettings = new IAnimationSettings[] {animationSettings}};

            Animator animator = creator.Create(sb1, new int[]{100}, null);
            TransformationSettings expectedSettings =
                animator.TransformationController.AnimationTransformation.MasterTransformationSettings;

            animator = creator.Create(sb2, new int[] { 100 }, null);
            Assert.IsTrue(ReferenceEquals(expectedSettings, animator.TransformationController.AnimationTransformation.MasterTransformationSettings));
            
        }
    }
}
