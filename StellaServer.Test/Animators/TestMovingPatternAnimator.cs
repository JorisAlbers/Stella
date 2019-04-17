using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using StellaLib.Animation;
using StellaServer.Animation.Animators;

namespace StellaServer.Test.Animators
{
    [TestFixture]
    public class TestMovingPatternAnimator
    {
        [Test]
        public void Create_pattern_CorrectlyCreatesListOfFrames()
        {
            Color expectedColor1 = Color.FromArgb(1, 2, 3);
            Color expectedColor2 = Color.FromArgb(4, 5, 6);
            Color expectedColor3 = Color.FromArgb(7, 8, 9);

            // Setup
            Color[] pattern = new Color[]
            {
                expectedColor1,
                expectedColor2,
                expectedColor3,
            };
            int lengthStrip = 4;
            int frameWaitMS = 100;
            MovingPatternAnimator animator = new MovingPatternAnimator(lengthStrip, frameWaitMS, pattern);
            List<Frame> frames = animator.Create();

            //Assert
            Assert.AreEqual(6, frames.Count); // slide in = 2, normal = 1 , slide out = 2
            //Frame 1, slide in
            Frame frame = frames[0];
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(0, frame.TimeStampRelative);
            Assert.AreEqual(expectedColor3, frame[0].Color);
            //Frame 2, slide in
            frame = frames[1];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(frameWaitMS, frame.TimeStampRelative);
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[0].Color);
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[1].Color);
            //Frame 3 , normal
            frame = frames[2];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(frameWaitMS * 2, frame.TimeStampRelative);
            Assert.AreEqual(expectedColor1, frame[0].Color );
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[1].Color );
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[2].Color );
            Assert.AreEqual(2, frame[2].Index);
            //Frame 4 , normal
            frame = frames[3];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(frameWaitMS * 3, frame.TimeStampRelative);
            Assert.AreEqual(1, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].Color);
            Assert.AreEqual(2, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].Color);
            Assert.AreEqual(3, frame[2].Index);
            Assert.AreEqual(expectedColor3, frame[2].Color);

            //Frame 5, slide out
            frame = frames[4];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(frameWaitMS * 4, frame.TimeStampRelative);
            Assert.AreEqual(2, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].Color);
            Assert.AreEqual(3, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].Color);
            //Frame 6, slide out
            frame = frames[5];
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(frameWaitMS * 5, frame.TimeStampRelative);
            Assert.AreEqual(3, frame[0].Index);
            Assert.AreEqual(expectedColor1,  frame[0].Color);
        }
    }
}
