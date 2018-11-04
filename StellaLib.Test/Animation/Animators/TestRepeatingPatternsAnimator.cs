using System.Collections.Generic;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Animation.Animators;

namespace StellaLib.Test.Animation.Animators
{
    [TestFixture]
    public class TestRepeatingPatternsAnimator
    {
        [Test]
        public void Create_singlePattern_CreatesCorrectFrameSet()
        {
            // Setup
            Color[] pattern = new Color[]{
                 new Color{ Red = 1, Green = 2, Blue = 3 },
                 new Color{ Red = 4, Green = 5, Blue = 6 },
                 new Color{ Red = 7, Green = 8, Blue = 9 },
                 };
            int lengthStrip = 7;
            RepeatingPatternsAnimator animator = new RepeatingPatternsAnimator(lengthStrip,new List<Color[]>{pattern});

            // Expected
            Color expectedColor1 = new Color{ Red = 1, Green = 2, Blue = 3 };
            Color expectedColor2 = new Color{ Red = 4, Green = 5, Blue = 6 };
            Color expectedColor3 = new Color{ Red = 7, Green = 8, Blue = 9 };

            FrameSet frameSet = animator.Create();

            //Assert
            Assert.AreEqual(1,frameSet.Count);
            Frame frame = frameSet[0];
            Assert.AreEqual(lengthStrip, frame.Count);
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
            patterns.Add(new Color[]{
                 new Color{ Red = 1, Green = 2, Blue = 3 },
                 });
            patterns.Add(new Color[]{
                 new Color{ Red = 4, Green = 5, Blue = 6 },
                 });

            int lengthStrip = 7;
            RepeatingPatternsAnimator animator = new RepeatingPatternsAnimator(lengthStrip, patterns);

            // Expected
            Color expectedColor1 = new Color{ Red = 1, Green = 2, Blue = 3 };
            Color expectedColor2 = new Color{ Red = 4, Green = 5, Blue = 6 };

            FrameSet frameSet = animator.Create();

            //Assert
            Assert.AreEqual(2,frameSet.Count);
            Frame frame1 = frameSet[0];
            Assert.AreEqual(frame1[0].Color, expectedColor1);
            Frame frame2 = frameSet[1];
            Assert.AreEqual(frame2[0].Color, expectedColor2);


        }
    }
}