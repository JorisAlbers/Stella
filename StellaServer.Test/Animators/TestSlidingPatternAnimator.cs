using System.Collections.Generic;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation.Animators;
using StellaLib.Network;
using System.Drawing;

namespace StellaServer.Test.Animation.Animators
{
    [TestFixture]
    public class TestSlidingPatternAnimator
    {
        [Test]
        public void Create_ThreeColors_CreatesCorrectFrames()
        {
            // Setup
            Color[] pattern = new Color[]
            {
                Color.FromArgb(1,2,3),
                Color.FromArgb(4,5,6),
                Color.FromArgb(7,8,9)
            };
            int lengthStrip = 7;
            int frameWaitMS = 100;
            SlidingPatternAnimator animator = new SlidingPatternAnimator(lengthStrip,frameWaitMS,pattern);

            // Expected
            Color expectedColor1 = Color.FromArgb(1,2,3);
            Color expectedColor2 = Color.FromArgb(4,5,6);
            Color expectedColor3 = Color.FromArgb(7,8,9);

            List<Frame> frames = animator.Create();

            //Assert
            Assert.AreEqual(3,frames.Count);
            //Frame 1
            Frame frame1 = frames[0];
            Assert.AreEqual(lengthStrip, frame1.Count);
            Assert.AreEqual(frameWaitMS, frame1.TimeStampRelative);
            Assert.AreEqual(frame1[0].Color, expectedColor1);
            Assert.AreEqual(frame1[1].Color, expectedColor2);
            Assert.AreEqual(frame1[2].Color, expectedColor3);
            Assert.AreEqual(frame1[3].Color, expectedColor1);
            Assert.AreEqual(frame1[4].Color, expectedColor2);
            Assert.AreEqual(frame1[5].Color, expectedColor3);
            Assert.AreEqual(frame1[6].Color, expectedColor1);
            //Frame 2
            Frame frame2 = frames[1];
            Assert.AreEqual(lengthStrip, frame2.Count);
            Assert.AreEqual(frameWaitMS, frame2.TimeStampRelative);
            Assert.AreEqual(frame2[0].Color, expectedColor2);
            Assert.AreEqual(frame2[1].Color, expectedColor3);
            Assert.AreEqual(frame2[2].Color, expectedColor1);
            Assert.AreEqual(frame2[3].Color, expectedColor2);
            Assert.AreEqual(frame2[4].Color, expectedColor3);
            Assert.AreEqual(frame2[5].Color, expectedColor1);
            Assert.AreEqual(frame2[6].Color, expectedColor2);
            //Frame 3
            Frame frame3 = frames[2];
            Assert.AreEqual(lengthStrip, frame3.Count);
            Assert.AreEqual(frameWaitMS, frame3.TimeStampRelative);
            Assert.AreEqual(frame3[0].Color, expectedColor3);
            Assert.AreEqual(frame3[1].Color, expectedColor1);
            Assert.AreEqual(frame3[2].Color, expectedColor2);
            Assert.AreEqual(frame3[3].Color, expectedColor3);
            Assert.AreEqual(frame3[4].Color, expectedColor1);
            Assert.AreEqual(frame3[5].Color, expectedColor2);
            Assert.AreEqual(frame3[6].Color, expectedColor3);
        }
    }
}