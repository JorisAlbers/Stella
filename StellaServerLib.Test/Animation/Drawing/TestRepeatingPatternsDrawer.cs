using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Test.Animation.Drawing
{
    [TestFixture]
    public class TestRepeatingPatternsDrawer
    {
        [Test]
        public void Create_singlePattern_CreatesCorrectFrames()
        {
            // Setup
            Color[] pattern = new Color[]{
                 Color.FromArgb(1,2,3),
                 Color.FromArgb(4,5,6),
                 Color.FromArgb(7,8,9)
                 };
            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(0,lengthStrip,frameWaitMS,new List<Color[]>{pattern});

            // Expected
            Color expectedColor1 = Color.FromArgb(1,2,3);
            Color expectedColor2 = Color.FromArgb(4,5,6);
            Color expectedColor3 = Color.FromArgb(7,8,9);

            Frame frame = drawer.First();

            //Assert
            Assert.AreEqual(lengthStrip, frame.Count);
            Assert.AreEqual(0, frame.TimeStampRelative);
            Assert.AreEqual(frame[0].Color, expectedColor1);
            Assert.AreEqual(frame[1].Color, expectedColor2);
            Assert.AreEqual(frame[2].Color, expectedColor3);
            Assert.AreEqual(frame[3].Color, expectedColor1);
            Assert.AreEqual(frame[4].Color, expectedColor2);
            Assert.AreEqual(frame[5].Color, expectedColor3);
            Assert.AreEqual(frame[6].Color, expectedColor1);
        }

        [Test]
        public void Create_singlePatternWithStartIndexOf100_CreatesCorrectFrames()
        {
            // Setup
            Color[] pattern = new Color[]{
                Color.FromArgb(1,2,3),
                Color.FromArgb(4,5,6),
                Color.FromArgb(7,8,9)
            };
            int startIndex = 100;
            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(startIndex,lengthStrip, frameWaitMS, new List<Color[]> { pattern });

            // Expected
            int expectedIndex1 = 100;
            int expectedIndex2 = 101;
            int expectedIndex3 = 102;
            int expectedIndex4 = 103;
            int expectedIndex5 = 104;
            int expectedIndex6 = 105;
            int expectedIndex7 = 106;

            Frame frame = drawer.First();

            //Assert
            Assert.AreEqual(lengthStrip, frame.Count);
            Assert.AreEqual(0, frame.TimeStampRelative);
            Assert.AreEqual(frame[0].Index, expectedIndex1);
            Assert.AreEqual(frame[1].Index, expectedIndex2);
            Assert.AreEqual(frame[2].Index, expectedIndex3);
            Assert.AreEqual(frame[3].Index, expectedIndex4);
            Assert.AreEqual(frame[4].Index, expectedIndex5);
            Assert.AreEqual(frame[5].Index, expectedIndex6);
            Assert.AreEqual(frame[6].Index, expectedIndex7);
        }

        [Test]
        public void Create_TwoPatterns_CreatesFrameListWithTwoFrames()
        {
            // Setup
            List<Color[]> patterns = new List<Color[]>();
            patterns.Add(new Color[] { Color.FromArgb(1,2,3) });
            patterns.Add(new Color[] { Color.FromArgb(4,5,6) });

            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(0,lengthStrip,frameWaitMS, patterns);

            // Expected
            Color expectedColor1 =  Color.FromArgb(1,2,3);
            Color expectedColor2 =  Color.FromArgb(4,5,6);

            List<Frame> frames = drawer.Take(2).ToList();


            //Assert
            Frame frame1 = frames[0];
            Assert.AreEqual(0, frame1.TimeStampRelative);
            Assert.AreEqual(frame1[0].Color, expectedColor1);
            Frame frame2 = frames[1];
            Assert.AreEqual(frameWaitMS, frame2.TimeStampRelative);
            Assert.AreEqual(frame2[0].Color, expectedColor2);
        }
    }
}