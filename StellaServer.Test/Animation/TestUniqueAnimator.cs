using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Test.Animation
{
    [TestFixture]
    public class TestUniqueAnimator
    {
        [Test]
        public void GetNextFrame_TwoPis_DifferentFramesReturned()
        {
            Frame expectedFrame1 = new Frame(0, 0)
            {
                new PixelInstruction(1, 10, 20, 30)
            };
            Frame expectedFrame2 = new Frame(1, 1)
            {
                new PixelInstruction(1, 10, 20, 30)
            };
            
            List<Frame> frames1 = new List<Frame>()
            {
                expectedFrame1,
            };
            List<Frame> frames2 = new List<Frame>()
            {
                expectedFrame2
            };

            DateTime[] dateTimes = new DateTime[] { DateTime.Now, DateTime.Now };
            
            var drawer1 = new Mock<IDrawer>();
            drawer1.Setup(x => x.Create()).Returns(frames1);
            var drawer2 = new Mock<IDrawer>();
            drawer2.Setup(x => x.Create()).Returns(frames2);

            UniqueAnimator animator = new UniqueAnimator(new IDrawer[]{drawer1.Object,drawer2.Object}, dateTimes);

            // Pi 1
            Assert.AreEqual(expectedFrame1, animator.GetNextFrame(0));
            // Pi 2
            Assert.AreEqual(expectedFrame2, animator.GetNextFrame(1));
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

            DateTime[] dateTimes = new DateTime[]{DateTime.Now };

            var drawer = new Mock<IDrawer>();
            drawer.Setup(x => x.Create()).Returns(frames);

            UniqueAnimator animator = new UniqueAnimator(new IDrawer[] { drawer.Object }, dateTimes);

            // Pi 1
            Assert.AreEqual(expectedFrame1, animator.GetNextFrame(0));
            Assert.AreEqual(expectedFrame2, animator.GetNextFrame(0));
            //  loop
            Assert.AreEqual(expectedFrame1, animator.GetNextFrame(0));
        }

        [Test]
        public void GetFrameSetMetadata_TwoPis_CorrectFrameSetMetadata()
        {
            Frame expectedFrame1 = new Frame(0, 0)
            {
                new PixelInstruction(1, 10, 20, 30)
            };
            Frame expectedFrame2 = new Frame(1, 1)
            {
                new PixelInstruction(1, 10, 20, 30)
            };

            DateTime expectedDateTime1 = DateTime.Now;
            DateTime expectedDateTime2 = DateTime.Now + TimeSpan.FromSeconds(1);


            List<Frame> frames = new List<Frame>()
            {
                expectedFrame1,
                expectedFrame2
            };

            var drawer = new Mock<IDrawer>();
            drawer.Setup(x => x.Create()).Returns(frames);

            UniqueAnimator animator = new UniqueAnimator(new IDrawer[] { drawer.Object, drawer.Object }, new DateTime[]{expectedDateTime1,expectedDateTime2});

            // Pi 1
            Assert.AreEqual(expectedDateTime1, animator.GetFrameSetMetadata(0).TimeStamp);
            Assert.AreEqual(expectedDateTime2, animator.GetFrameSetMetadata(1).TimeStamp);
        }
    }
}
