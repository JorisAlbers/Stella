using System.Collections.Generic;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation.Animators;
using StellaLib.Network;
using System.Drawing;

namespace StellaServer.Test.Animation.Animators
{
    [TestFixture]
    public class TestRepeatingPatternsAnimator
    {
        [Test]
        public void Create_singlePattern_CreatesCorrectFrameSet()
        {
            // Setup
            Color[] pattern = new Color[]{
                 Color.FromArgb(1,2,3),
                 Color.FromArgb(4,5,6),
                 Color.FromArgb(7,8,9)
                 };
            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsAnimator animator = new RepeatingPatternsAnimator(lengthStrip,frameWaitMS,new List<Color[]>{pattern});

            // Expected
            Color expectedColor1 = Color.FromArgb(1,2,3);
            Color expectedColor2 = Color.FromArgb(4,5,6);
            Color expectedColor3 = Color.FromArgb(7,8,9);

            FrameSet frameSet = animator.Create();

            //Assert
            Assert.AreEqual(1,frameSet.Count);
            Frame frame = frameSet[0];
            Assert.AreEqual(lengthStrip, frame.Count);
            Assert.AreEqual(frameWaitMS, frame.WaitMS);
            Assert.AreEqual(frame[0].Color, expectedColor1);
            Assert.AreEqual(frame[1].Color, expectedColor2);
            Assert.AreEqual(frame[2].Color, expectedColor3);
            Assert.AreEqual(frame[3].Color, expectedColor1);
            Assert.AreEqual(frame[4].Color, expectedColor2);
            Assert.AreEqual(frame[5].Color, expectedColor3);
            Assert.AreEqual(frame[6].Color, expectedColor1);
        }

        [Test]
        public void Create_TwoPatterns_CreatesFrameSetWithTwoFrames()
        {
            // Setup
            List<Color[]> patterns = new List<Color[]>();
            patterns.Add(new Color[] { Color.FromArgb(1,2,3) });
            patterns.Add(new Color[] { Color.FromArgb(4,5,6) });

            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsAnimator animator = new RepeatingPatternsAnimator(lengthStrip,frameWaitMS, patterns);

            // Expected
            Color expectedColor1 =  Color.FromArgb(1,2,3);
            Color expectedColor2 =  Color.FromArgb(4,5,6);

            FrameSet frameSet = animator.Create();

            //Assert
            Assert.AreEqual(2,frameSet.Count);
            Frame frame1 = frameSet[0];
            Assert.AreEqual(frameWaitMS, frame1.WaitMS);
            Assert.AreEqual(frame1[0].Color, expectedColor1);
            Frame frame2 = frameSet[1];
            Assert.AreEqual(frameWaitMS, frame2.WaitMS);
            Assert.AreEqual(frame2[0].Color, expectedColor2);
        }
    }
}