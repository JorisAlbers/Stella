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
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(lengthStrip,frameWaitMS,new List<Color[]>{pattern});

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
        public void Create_TwoPatterns_CreatesFrameListWithTwoFrames()
        {
            // Setup
            List<Color[]> patterns = new List<Color[]>();
            patterns.Add(new Color[] { Color.FromArgb(1,2,3) });
            patterns.Add(new Color[] { Color.FromArgb(4,5,6) });

            int lengthStrip = 7;
            int frameWaitMS = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(lengthStrip,frameWaitMS, patterns);

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