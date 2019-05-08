using System.Collections.Generic;
using System.Drawing;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Test.Drawing
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

            Color expectedColor1 = Color.FromArgb(255, 255, 0, 0);
            Color expectedColor2 = Color.FromArgb(255, 0, 255, 0);
            Color expectedColor3 = Color.FromArgb(255, 0, 0, 255);

           
            Bitmap bitmap = new Bitmap(width,height);
            bitmap.SetPixel(0,0,expectedColor1);
            bitmap.SetPixel(1,0,expectedColor2);
            bitmap.SetPixel(2,0,expectedColor3);
            
            // ACT
            BitmapDrawer drawer = new BitmapDrawer(stripLength,frameWaitMs,bitmap);
            List<Frame> frames = drawer.Create();

            // ASSERT
            Assert.AreEqual(height, frames.Count);
            Assert.AreEqual(expectedColor1, frames[0][0].Color);
            Assert.AreEqual(expectedColor2, frames[0][1].Color);
            Assert.AreEqual(expectedColor3, frames[0][2].Color);
        }

        [Test]
        public void Create_BitmapWithTwoRow_CreatesAnimationWithOneFrame()
        {
            // SETUP
            int width = 3;
            int height = 2;

            int stripLength = 3;
            int frameWaitMs = 100;

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
            BitmapDrawer drawer = new BitmapDrawer(stripLength, frameWaitMs, bitmap);
            List<Frame> frames = drawer.Create();

            // ASSERT
            Assert.AreEqual(height, frames.Count);
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

    }
}
