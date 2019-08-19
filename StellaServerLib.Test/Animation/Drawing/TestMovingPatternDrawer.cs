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
            int frameWaitMS = 100;
            int framesToTake = 6;
            MovingPatternDrawer drawer = new MovingPatternDrawer(0,lengthStrip, pattern);
            List<List<PixelInstructionWithDelta>> frames = drawer.Take(framesToTake).ToList();

            //Assert
            //Frame 1, slide in
            List<PixelInstructionWithDelta> frame = frames[0];
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(expectedColor3, frame[0].ToColor());
            //Frame 2, slide in
            frame = frames[1];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[0].ToColor());
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[1].ToColor());
            //Frame 3 , normal
            frame = frames[2];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(expectedColor1, frame[0].ToColor() );
            Assert.AreEqual(0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor() );
            Assert.AreEqual(1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor() );
            Assert.AreEqual(2, frame[2].Index);
            //Frame 4 , normal
            frame = frames[3];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(1, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(2, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(3, frame[2].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());

            //Frame 5, slide out
            frame = frames[4];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(2, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(3, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            //Frame 6, slide out
            frame = frames[5];
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
            int frameWaitMS = 100;
            int framesToTake = 6;
            MovingPatternDrawer drawer = new MovingPatternDrawer(startIndex, lengthStrip,  pattern);
            List<List<PixelInstructionWithDelta>> frames = drawer.Take(framesToTake).ToList();

            //Assert
            //Frame 1, slide in
            List<PixelInstructionWithDelta> frame = frames[0];
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(expectedColor3, frame[0].ToColor());
            //Frame 2, slide in
            frame = frames[1];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(startIndex + 0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[0].ToColor());
            Assert.AreEqual(startIndex + 1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[1].ToColor());
            //Frame 3 , normal
            frame = frames[2];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 0, frame[0].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(startIndex + 1, frame[1].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());
            Assert.AreEqual(startIndex + 2, frame[2].Index);
            //Frame 4 , normal
            frame = frames[3];
            Assert.AreEqual(3, frame.Count);
            Assert.AreEqual(startIndex + 1, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 2, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            Assert.AreEqual(startIndex + 3, frame[2].Index);
            Assert.AreEqual(expectedColor3, frame[2].ToColor());

            //Frame 5, slide out
            frame = frames[4];
            Assert.AreEqual(2, frame.Count);
            Assert.AreEqual(startIndex + 2, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
            Assert.AreEqual(startIndex + 3, frame[1].Index);
            Assert.AreEqual(expectedColor2, frame[1].ToColor());
            //Frame 6, slide out
            frame = frames[5];
            Assert.AreEqual(1, frame.Count);
            Assert.AreEqual(startIndex + 3, frame[0].Index);
            Assert.AreEqual(expectedColor1, frame[0].ToColor());
        }
    }
}
