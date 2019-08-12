using System;
using System.Collections.Generic;
using System.Text;
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
        public void GetCorrectedFrameWaitMs_Index_CorrectValue(int masterFrameWaitMs, int animationFrameWaitMs)
        {
            int expected = masterFrameWaitMs + animationFrameWaitMs;
            AnimationTransformation animationTransformation = new AnimationTransformation(new TransformationSettings(0));
            animationTransformation.AddTransformationSettings(animationFrameWaitMs);
            animationTransformation.SetFrameWaitMs(masterFrameWaitMs);
            Assert.AreEqual(expected, animationTransformation.GetCorrectedFrameWaitMs(0));
        }
    }
}
