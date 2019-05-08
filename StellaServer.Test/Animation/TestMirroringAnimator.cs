using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Test.Animation
{
    [TestFixture]
    class TestMirroringAnimator
    {
        [Test]
        public void GetNextFrame_TwoPis_SameFrame()
        {
            Frame expectedFrame1 = new Frame(0, 0)
            {
                new PixelInstruction(1, 10, 20, 30)
            };
            Frame expectedFrame2 = new Frame(1, 1)
            {
                new PixelInstruction(1, 10, 20, 30)
            };


            List<Frame> frames = new List<Frame>()
            {
               expectedFrame1,
               expectedFrame2
            };

            var drawer = new Mock<IDrawer>();
            drawer.Setup(x => x.Create()).Returns(frames);

            MirroringAnimator animator = new MirroringAnimator(drawer.Object,2);

            // Pi 1
            Assert.AreEqual(expectedFrame1,animator.GetNextFrame(0));
            Assert.AreEqual(expectedFrame2,animator.GetNextFrame(0));
            // Pi 2
            Assert.AreEqual(expectedFrame1,animator.GetNextFrame(1));
            Assert.AreEqual(expectedFrame2,animator.GetNextFrame(1));
        }

        [Test]
        public void GetNextFrame_MoreThanFramesRequestedThanDrawerReturns_Loops()
        {
            Frame expectedFrame1 = new Frame(0, 0)
            {
                new PixelInstruction(1, 10, 20, 30)
            };
            Frame expectedFrame2 = new Frame(1, 1)
            {
                new PixelInstruction(1, 10, 20, 30)
            };


            List<Frame> frames = new List<Frame>()
            {
                expectedFrame1,
                expectedFrame2
            };

            var drawer = new Mock<IDrawer>();
            drawer.Setup(x => x.Create()).Returns(frames);

            MirroringAnimator animator = new MirroringAnimator(drawer.Object, 1);

            // Pi 1
            Assert.AreEqual(expectedFrame1, animator.GetNextFrame(0));
            Assert.AreEqual(expectedFrame2, animator.GetNextFrame(0));
            //  loop
            Assert.AreEqual(expectedFrame1, animator.GetNextFrame(0));
        }
    }
}
