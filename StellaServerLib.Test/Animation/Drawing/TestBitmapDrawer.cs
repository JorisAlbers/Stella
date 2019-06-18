using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Test.Animation.Drawing
{
    [TestFixture]
    class TestBitmapDrawer
    {
        [Test]
        public void Create_BitmapWithOneRow_CreatesAnimationWithOneFrame()
        {
            // SETUP
            int width = 3;
            int height = 1;

            int stripLength = 3;
            int frameWaitMs = 100;
            int startIndex = 0;

            Color expectedColor1 = Color.FromArgb(255, 255, 0, 0);
            Color expectedColor2 = Color.FromArgb(255, 0, 255, 0);
            Color expectedColor3 = Color.FromArgb(255, 0, 0, 255);

           
            Bitmap bitmap = new Bitmap(width,height);
            bitmap.SetPixel(0,0,expectedColor1);
            bitmap.SetPixel(1,0,expectedColor2);
            bitmap.SetPixel(2,0,expectedColor3);
            
            // ACT
            BitmapDrawer drawer = new BitmapDrawer(startIndex, stripLength,frameWaitMs,bitmap);

            Frame frame = drawer.First();

            // ASSERT
            Assert.AreEqual(expectedColor1, frame[0].Color);
            Assert.AreEqual(expectedColor2, frame[1].Color);
            Assert.AreEqual(expectedColor3, frame[2].Color);
        }

        [Test]
        public void Create_BitmapWithTwoRow_CreatesAnimationWithOneFrame()
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
            BitmapDrawer drawer = new BitmapDrawer(startIndex,stripLength, frameWaitMs, bitmap);
            List<Frame> frames = drawer.Take(2).ToList();

            // ASSERT
            // row 1
            Assert.AreEqual(3, frames[0].Count);
            Assert.AreEqual(expectedColor1, frames[0][0].Color);
            Assert.AreEqual(expectedColor2, frames[0][1].Color);
            Assert.AreEqual(expectedColor3, frames[0][2].Color);
            // row 2
            Assert.AreEqual(3,frames[1].Count);
            Assert.AreEqual(expectedColor4, frames[1][0].Color);
            Assert.AreEqual(expectedColor5, frames[1][1].Color);
            Assert.AreEqual(expectedColor6, frames[1][2].Color);

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
            BitmapDrawer drawer = new BitmapDrawer(startIndex, stripLength, frameWaitMs, bitmap);

            Frame frame = drawer.First();

            // ASSERT
            Assert.AreEqual(expectedIndex1, frame[0].Index);
            Assert.AreEqual(expectedIndex2, frame[1].Index);
            Assert.AreEqual(expectedIndex3, frame[2].Index);
        }

    }

    [TestFixture]
    class TestBitmapDrawerWithoutDelta
    {
        [Test]
        public void Create_BitmapWithOneRow_CreatesAnimationWithOneFrame()
        {
            // SETUP
            int width = 3;
            int height = 1;

            int stripLength = 3;
            int frameWaitMs = 100;

            Color expectedColor1 = Color.FromArgb(255, 255, 0, 0);
            Color expectedColor2 = Color.FromArgb(255, 0, 255, 0);
            Color expectedColor3 = Color.FromArgb(255, 0, 0, 255);


            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetPixel(0, 0, expectedColor1);
            bitmap.SetPixel(1, 0, expectedColor2);
            bitmap.SetPixel(2, 0, expectedColor3);

            // ACT
            BitmapDrawerWithoutDelta drawer = new BitmapDrawerWithoutDelta(stripLength, frameWaitMs, bitmap);

            FrameWithoutDelta frame = drawer.First();

            // ASSERT
            Assert.AreEqual(expectedColor1, frame[0].Color);
            Assert.AreEqual(expectedColor2, frame[1].Color);
            Assert.AreEqual(expectedColor3, frame[2].Color);
        }

        [Test]
        public void Create_BitmapWithTwoRow_CreatesAnimationWithOneFrame()
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
            Color expectedColor5 = Color.FromArgb(255, 255, 0, 255);
            Color expectedColor6 = Color.FromArgb(255, 0, 0, 0);


            Bitmap bitmap = new Bitmap(width, height);
            bitmap.SetPixel(0, 0, expectedColor1);
            bitmap.SetPixel(1, 0, expectedColor2);
            bitmap.SetPixel(2, 0, expectedColor3);
            bitmap.SetPixel(0, 1, expectedColor4);
            bitmap.SetPixel(1, 1, expectedColor5);
            bitmap.SetPixel(2, 1, expectedColor6);

            // ACT
            BitmapDrawerWithoutDelta drawer = new BitmapDrawerWithoutDelta(stripLength, frameWaitMs, bitmap);
            List<FrameWithoutDelta> frames = drawer.Take(2).ToList();

            // ASSERT
            // row 1
            Assert.AreEqual(3, frames[0].Count);
            Assert.AreEqual(expectedColor1, frames[0][0].Color);
            Assert.AreEqual(expectedColor2, frames[0][1].Color);
            Assert.AreEqual(expectedColor3, frames[0][2].Color);
            // row 2
            Assert.AreEqual(3, frames[1].Count);
            Assert.AreEqual(expectedColor4, frames[1][0].Color);
            Assert.AreEqual(expectedColor5, frames[1][1].Color);
            Assert.AreEqual(expectedColor6, frames[1][2].Color);

        }
    }
}
