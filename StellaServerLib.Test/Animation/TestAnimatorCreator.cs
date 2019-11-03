using System.Drawing;
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
            AnimatorCreator creator = new AnimatorCreator(new FrameProviderCreator(null,1));

            MovingPatternAnimationSettings animationSettings = new MovingPatternAnimationSettings
            {
                TimeUnitsPerFrame = 10,
                Pattern = new Color[] {Color.Red},
                RelativeStart = 10,
                StartIndex = 0,
                StripLength = 100
            };


            Storyboard sb1 = new Storyboard {AnimationSettings = new IAnimationSettings[] {animationSettings}};
            Storyboard sb2 = new Storyboard {AnimationSettings = new IAnimationSettings[] {animationSettings}};

            IAnimator animator = creator.Create(sb1, new int[]{100}, null);
            TransformationSettings expectedSettings =
                animator.TransformationController.AnimationTransformation.MasterTransformationSettings;

            animator = creator.Create(sb2, new int[] { 100 }, null);
            Assert.IsTrue(ReferenceEquals(expectedSettings, animator.TransformationController.AnimationTransformation.MasterTransformationSettings));
            
        }
    }
}
