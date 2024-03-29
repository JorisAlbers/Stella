using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Test.Animation.Drawing
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
            int timeUnitsPerFrame = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(0,lengthStrip,new Color[][]{pattern});

            // Expected
            Color expectedColor1 = Color.FromArgb(1,2,3);
            Color expectedColor2 = Color.FromArgb(4,5,6);
            Color expectedColor3 = Color.FromArgb(7,8,9);

            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame = drawer.Current;

            //Assert
            Assert.AreEqual(lengthStrip, frame.Count);
            Assert.AreEqual(frame[0].ToColor(), expectedColor1);
            Assert.AreEqual(frame[1].ToColor(), expectedColor2);
            Assert.AreEqual(frame[2].ToColor(), expectedColor3);
            Assert.AreEqual(frame[3].ToColor(), expectedColor1);
            Assert.AreEqual(frame[4].ToColor(), expectedColor2);
            Assert.AreEqual(frame[5].ToColor(), expectedColor3);
            Assert.AreEqual(frame[6].ToColor(), expectedColor1);
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
            int timeUnitsPerFrame = 100;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(startIndex,lengthStrip,  new Color[][] { pattern });

            // Expected
            int expectedIndex1 = 100;
            int expectedIndex2 = 101;
            int expectedIndex3 = 102;
            int expectedIndex4 = 103;
            int expectedIndex5 = 104;
            int expectedIndex6 = 105;
            int expectedIndex7 = 106;

            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame = drawer.Current;

            //Assert
            Assert.AreEqual(lengthStrip, frame.Count);
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
            Color[][] patterns = new Color[][]
            {
                new Color[] {Color.FromArgb(1, 2, 3)},
                new Color[] {Color.FromArgb(4, 5, 6)}

            };

            int lengthStrip = 7;
            RepeatingPatternsDrawer drawer = new RepeatingPatternsDrawer(0,lengthStrip, patterns);

            // Expected
            Color expectedColor1 =  Color.FromArgb(1,2,3);
            Color expectedColor2 =  Color.FromArgb(4,5,6);

            //Assert
            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame1 = drawer.Current;
            Assert.AreEqual(frame1[0].ToColor(), expectedColor1);
            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame2 = drawer.Current;
            Assert.AreEqual(frame2[0].ToColor(), expectedColor2);
        }
    }
}