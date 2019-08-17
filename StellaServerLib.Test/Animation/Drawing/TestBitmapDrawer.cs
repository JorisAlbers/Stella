using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Test.Animation.Drawing
{
    [TestFixture]
    class TestBitmapDrawer
    {
        [Test]
        public void CreateFrames_BitmapWithOneRow_CreatesAnimationWithOneFrame()
        {
            // SETUP
            int width = 3;
            int height = 1;

            int stripLength = 3;
            int frameWaitMs = 100;
            int startIndex = 0;

            Color expectedColor1 = Color.FromArgb(255, 0, 0);
            Color expectedColor2 = Color.FromArgb(0, 255, 0);
            Color expectedColor3 = Color.FromArgb(0, 0, 255);

           
            Bitmap bitmap = new Bitmap(width,height);
            bitmap.SetPixel(0,0,expectedColor1);
            bitmap.SetPixel(1,0,expectedColor2);
            bitmap.SetPixel(2,0,expectedColor3);
            
            // ACT
            List<PixelInstructionWithoutDelta>[] frames = BitmapDrawer.CreateFrames(bitmap);

            // ASSERT
            Assert.AreEqual(1,frames.Length);
            Assert.AreEqual(3, frames[0].Count);
            Assert.AreEqual(expectedColor1, frames[0][0].ToColor());
            Assert.AreEqual(expectedColor2, frames[0][1].ToColor());
            Assert.AreEqual(expectedColor3, frames[0][2].ToColor());
        }

        [Test]
        public void CreateFrames_BitmapWithTwoRow_CreatesAnimationWithOneFrame()
        {
            // SETUP
            int width = 3;
            int height = 2;

            int stripLength = 3;
            int frameWaitMs = 100;
            int startIndex = 0;
            
            Color expectedColor1 = Color.FromArgb(255, 255, 0, 0);
            Color expectedColor2 = Color.FromArgb(255, 0, 255, 0);
            Color expectedColor3 = Color.FromArgb(255, 0, 0, 255);

            Color expectedColor4 = Color.FromArgb(255, 255, 255, 255);
            Color expectedColor5 =  Color.FromArgb(255, 255, 0, 255);
            Color expectedColor6 =  Color.FromArgb(255, 0, 0, 0);


            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetPixel(0, 0, expectedColor1);
            bitmap.SetPixel(1, 0, expectedColor2);
            bitmap.SetPixel(2, 0, expectedColor3);
            bitmap.SetPixel(0, 1, expectedColor4);
            bitmap.SetPixel(1, 1, expectedColor5);
            bitmap.SetPixel(2, 1, expectedColor6);

            // ACT
            List<PixelInstructionWithoutDelta>[] frames = BitmapDrawer.CreateFrames(bitmap);

            // ASSERT
            Assert.AreEqual(2,frames.Length);
            // row 1
            Assert.AreEqual(3, frames[0].Count);
            Assert.AreEqual(expectedColor1, frames[0][0].ToColor());
            Assert.AreEqual(expectedColor2, frames[0][1].ToColor());
            Assert.AreEqual(expectedColor3, frames[0][2].ToColor());
            // row 2
            Assert.AreEqual(3,frames[1].Count);
            Assert.AreEqual(expectedColor4, frames[1][0].ToColor());
            Assert.AreEqual(expectedColor5, frames[1][1].ToColor());
            Assert.AreEqual(expectedColor6, frames[1][2].ToColor());

        }

        [Test]
        public void Create_BitmapWithOneRowAndStartIndexOf100_CreatesAnimationWithCorrectStartIndex()
        {
            // SETUP
            int width = 3;
            int height = 1;

            int stripLength = 3;
            int frameWaitMs = 100;
            int startIndex = 100;

            int expectedIndex1 = 100;
            int expectedIndex2 = 101;
            int expectedIndex3 = 102;

            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetPixel(0, 0, Color.FromArgb(255, 255, 0, 0));
            bitmap.SetPixel(1, 0, Color.FromArgb(255, 0, 255, 0));
            bitmap.SetPixel(2, 0, Color.FromArgb(255, 0, 0, 255));

            // ACT
            BitmapDrawer drawer = new BitmapDrawer(startIndex, stripLength, true, BitmapDrawer.CreateFrames(bitmap));

            List<PixelInstruction> frame = drawer.First();

            // ASSERT
            Assert.AreEqual(expectedIndex1, frame[0].Index);
            Assert.AreEqual(expectedIndex2, frame[1].Index);
            Assert.AreEqual(expectedIndex3, frame[2].Index);
        }

        [Test]
        public void Create_NoWrapping_CreatesAnimationThatDoesNotWrap()
        {
            // SETUP
            int width = 1;
            int height = 3;

            int stripLength = 3;
            int frameWaitMs = 100;
            int startIndex = 0;

            Color expectedColor1 = Color.FromArgb(255, 255, 0, 0);
            Color expectedColor2 = Color.FromArgb(255, 0, 255, 0);
            Color expectedColor3 = Color.FromArgb(255, 0, 0, 255);


            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetPixel(0, 0, expectedColor1);
            bitmap.SetPixel(0, 1, expectedColor2);
            bitmap.SetPixel(0, 2, expectedColor3);

            // ACT
            BitmapDrawer drawer = new BitmapDrawer(startIndex, stripLength, true, BitmapDrawer.CreateFrames(bitmap));
            List<List<PixelInstruction>> frames = drawer.Take(4).ToList();

            // ASSERT
            // row 1
            Assert.AreEqual(1, frames[0].Count);
            Assert.AreEqual(expectedColor1, frames[0][0].ToColor());
            // row 2
            Assert.AreEqual(1, frames[1].Count);
            Assert.AreEqual(expectedColor2, frames[1][0].ToColor());
            // Row 3
            Assert.AreEqual(1, frames[2].Count);
            Assert.AreEqual(expectedColor3, frames[2][0].ToColor());
            // Row 4 
            Assert.AreEqual(1, frames[3].Count);
            Assert.AreEqual(expectedColor3, frames[3][0].ToColor());

        }

    }
}
