using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServer.Test.Animation.Drawing
{
    [TestFixture]
    public class TestSlidingPatternDrawer
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
            int framesToTake = 3;
            SlidingPatternDrawer drawer = new SlidingPatternDrawer(0,lengthStrip,frameWaitMS,pattern);

            // Expected
            Color expectedColor1 = Color.FromArgb(1,2,3);
            Color expectedColor2 = Color.FromArgb(4,5,6);
            Color expectedColor3 = Color.FromArgb(7,8,9);

            List<Frame> frames = drawer.Take(framesToTake).ToList();

            //Assert
            //Frame 1
            Frame frame1 = frames[0];
            Assert.AreEqual(lengthStrip, frame1.Count);
            Assert.AreEqual(0, frame1.TimeStampRelative);
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
            Assert.AreEqual(frameWaitMS * 2, frame3.TimeStampRelative);
            Assert.AreEqual(frame3[0].Color, expectedColor3);
            Assert.AreEqual(frame3[1].Color, expectedColor1);
            Assert.AreEqual(frame3[2].Color, expectedColor2);
            Assert.AreEqual(frame3[3].Color, expectedColor3);
            Assert.AreEqual(frame3[4].Color, expectedColor1);
            Assert.AreEqual(frame3[5].Color, expectedColor2);
            Assert.AreEqual(frame3[6].Color, expectedColor3);
        }

        [Test]
        public void Create_StartIndexOf100_CreatesCorrectFrames()
        {
            // Setup
            Color[] pattern = new Color[]
            {
                Color.FromArgb(1,2,3),
            };
            int lengthStrip = 3;
            int frameWaitMS = 100;
            int framesToTake = 1;
            int startIndex = 100;
            SlidingPatternDrawer drawer = new SlidingPatternDrawer(startIndex,lengthStrip,frameWaitMS,pattern);

            // Expected
            int expectedIndex1 = 100;
            int expectedIndex2 = 101;
            int expectedIndex3 = 102;

            List<Frame> frames = drawer.Take(framesToTake).ToList();

            //Assert
            //Frame 1
            Frame frame1 = frames[0];
            Assert.AreEqual(lengthStrip, frame1.Count);
            Assert.AreEqual(0, frame1.TimeStampRelative);
            Assert.AreEqual(frame1[0].Index, expectedIndex1);
            Assert.AreEqual(frame1[1].Index, expectedIndex2);
            Assert.AreEqual(frame1[2].Index, expectedIndex3);
           
        }
    }
}