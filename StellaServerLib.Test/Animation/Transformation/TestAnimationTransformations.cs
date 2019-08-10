using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Test.Animation.Transformation
{
    [TestFixture]
    public class TestAnimationTransformations
    {
        [TestCase(5, 10)]
        [TestCase(10, 5)]
        [TestCase(10, 10)]
        public void GetCorrectedFrameWaitMs_Index_CorrectValue(int masterFrameWaitMs, int animationFrameWaitMs)
        {
            int expected = masterFrameWaitMs + animationFrameWaitMs;
            AnimationTransformations animationTransformations = new AnimationTransformations();
            animationTransformations.AddTransformationSettings(animationFrameWaitMs);
            animationTransformations.SetFrameWaitMs(masterFrameWaitMs);
            Assert.AreEqual(expected, animationTransformations.GetCorrectedFrameWaitMs(0));
        }
    }
}
