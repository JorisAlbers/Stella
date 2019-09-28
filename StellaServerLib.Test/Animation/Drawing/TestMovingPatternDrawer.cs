using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Test.Animation.Drawing
{
    [TestFixture]
    public class TestMovingPatternDrawer
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
            MovingPatternDrawer drawer = new MovingPatternDrawer(0,lengthStrip, pattern);

            //Assert
            //Frame 1, slide in
            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame = drawer.Current;
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(expectedColor3, frame[0].ToColor());
            //Frame 2, slide in
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[0].ToColor());
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[1].ToColor());
            //Frame 3 , normal
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(expectedColor1, frame[0].ToColor() );
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor() );
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor() );
            Assert.AreEqual(2, frame[2].Index);
            //Frame 4 , normal
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(1, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(2, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(3, frame[2].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());

            //Frame 5, slide out
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(2, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(3, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            //Frame 6, slide out
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(3, frame[0].Index);
            Assert.AreEqual(expectedColor1,  frame[0].ToColor());
        }


        [Test]
        public void Create_StartIndex_CorrectlyCreatesListOfFrames()
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
            int startIndex = 10;
            int lengthStrip = 4;
            MovingPatternDrawer drawer = new MovingPatternDrawer(startIndex, lengthStrip,  pattern);

            //Assert
            //Frame 1, slide in
            drawer.MoveNext();
            List<PixelInstructionWithDelta> frame = drawer.Current;
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(expectedColor3, frame[0].ToColor());
            //Frame 2, slide in
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(startIndex + 0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[0].ToColor());
            Assert.AreEqual(startIndex + 1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[1].ToColor());
            //Frame 3 , normal
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(startIndex + 1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());
            Assert.AreEqual(startIndex + 2, frame[2].Index);
            //Frame 4 , normal
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(startIndex + 1, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 2, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(startIndex + 3, frame[2].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());

            //Frame 5, slide out
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(startIndex + 2, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 3, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            //Frame 6, slide out
            drawer.MoveNext();
            frame = drawer.Current;
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(startIndex + 3, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
        }
    }
}
