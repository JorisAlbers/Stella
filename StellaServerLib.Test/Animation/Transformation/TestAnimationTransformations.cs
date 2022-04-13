using NUnit.Framework;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Test.Animation.Transformation
{
    [TestFixture]
    public class TestAnimationTransformation
    {
        [TestCase(5, 10)]
        [TestCase(10, 5)]
        [TestCase(10, 10)]
        public void GetCorrectedTimeUnitsPerFrame_Index_CorrectValue(int masterTimeUnitsPerFrame, int animationTimeUnitsPerFrame)
        {
            int expected = masterTimeUnitsPerFrame + animationTimeUnitsPerFrame;
            AnimationTransformation animationTransformation = new AnimationTransformation(new AnimationTransformationSettings(masterTimeUnitsPerFrame, 0,new float[3], false), new AnimationTransformationSettings[]
            {
                new AnimationTransformationSettings(animationTimeUnitsPerFrame,0,new float[3], false), 
            });
            
            Assert.AreEqual(expected, animationTransformation.GetCorrectedTimeUnitsPerFrame(0));
        }
    }
}
