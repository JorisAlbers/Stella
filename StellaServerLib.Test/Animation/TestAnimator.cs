using System;
using System.Collections.Generic;
using System.Drawing;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Test.Animation
{
    [TestFixture]
    public class TestAnimator
    {
        [Test]
        public void GetNextFramePerPi_OnePiMapping_ReturnsFrameForOnePi()
        {
            Color expectedColor = Color.FromArgb(1, 2, 3);
            int piIndex = 0;
            int expectedPixelIndex = 20;
            int expectedFrameIndex = 1;
            int expectedRelativeTimeStamp = 10;

            List<Frame> frames = new List<Frame>
            {
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstruction(0,expectedColor),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(frames.GetEnumerator());
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex , expectedPixelIndex)
            };

            Animator animator = new Animator(drawerMock.Object, mask, DateTime.Now);
            Frame[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(1, framePerPi.Length);

            Frame frame = framePerPi[0];
            Assert.AreEqual(expectedFrameIndex, frame.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex, frame[0].Index);
            Assert.AreEqual(expectedColor, frame[0].Color);
           
        }

        [Test]
        public void GetNextFramePerPi_ThreePiMappings_ReturnsFrameForEachPi()
        {
            Color expectedColor1 = Color.FromArgb(1, 2, 3);
            Color expectedColor2 = Color.FromArgb(4, 5, 6);
            Color expectedColor3 = Color.FromArgb(7, 8, 9);

            int piIndex1 = 0;
            int piIndex2 = 1;
            int piIndex3 = 2;

            int expectedPixelIndex1 = 20;
            int expectedPixelIndex2 = 30;
            int expectedPixelIndex3 = 40;

            int expectedFrameIndex = 1;
            int expectedRelativeTimeStamp = 10;


            List<Frame> frames = new List<Frame>
            {
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstruction(0,expectedColor1),
                    new PixelInstruction(1,expectedColor2),
                    new PixelInstruction(2,expectedColor3),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(frames.GetEnumerator());
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex1 , expectedPixelIndex1),
                new PiMaskItem(piIndex2 , expectedPixelIndex2),
                new PiMaskItem(piIndex3 , expectedPixelIndex3),
            };

            Animator animator = new Animator(drawerMock.Object,mask,DateTime.Now);
            Frame[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(3,framePerPi.Length);
            // Pi1
            Frame frame1 = framePerPi[0];
            Assert.AreEqual(expectedFrameIndex,frame1.Index);
            Assert.AreEqual(expectedRelativeTimeStamp,frame1.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex1,frame1[0].Index);
            Assert.AreEqual(expectedColor1,frame1[0].Color);
            // Pi1
            Frame frame2 = framePerPi[1];
            Assert.AreEqual(expectedFrameIndex, frame2.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame2.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex2, frame2[0].Index);
            Assert.AreEqual(expectedColor2, frame2[0].Color);
            // Pi 3
            Frame frame3 = framePerPi[2];
            Assert.AreEqual(expectedFrameIndex, frame3.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame3.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex3, frame3[0].Index);
            Assert.AreEqual(expectedColor3, frame3[0].Color);
        }

        [Test]
        public void GetNextFramePerPi_ThreePiMappings_ReturnsFrameForEachPiAtThirdFrame()
        {
            Color expectedColor1 = Color.FromArgb(1, 2, 3);
            Color expectedColor2 = Color.FromArgb(4, 5, 6);
            Color expectedColor3 = Color.FromArgb(7, 8, 9);

            int piIndex1 = 0;
            int piIndex2 = 1;
            int piIndex3 = 2;

            int expectedPixelIndex1 = 20;
            int expectedPixelIndex2 = 30;
            int expectedPixelIndex3 = 40;

            int expectedFrameIndex = 2;
            int expectedRelativeTimeStamp = 10;


            List<Frame> frames = new List<Frame>
            {
                new Frame(0,1),
                new Frame(1,2),
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstruction(0,expectedColor1),
                    new PixelInstruction(1,expectedColor2),
                    new PixelInstruction(2,expectedColor3),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(frames.GetEnumerator());
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex1 , expectedPixelIndex1),
                new PiMaskItem(piIndex2 , expectedPixelIndex2),
                new PiMaskItem(piIndex3 , expectedPixelIndex3),
            };

            Animator animator = new Animator(drawerMock.Object, mask, DateTime.Now);
            // Flush first two frames
            animator.GetNextFramePerPi();
            animator.GetNextFramePerPi();

            // Assert
            Frame[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(3, framePerPi.Length);
            // Pi1
            Frame frame1 = framePerPi[0];
            Assert.AreEqual(expectedFrameIndex, frame1.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame1.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex1, frame1[0].Index);
            Assert.AreEqual(expectedColor1, frame1[0].Color);
            // Pi1
            Frame frame2 = framePerPi[1];
            Assert.AreEqual(expectedFrameIndex, frame2.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame2.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex2, frame2[0].Index);
            Assert.AreEqual(expectedColor2, frame2[0].Color);
            // Pi 3
            Frame frame3 = framePerPi[2];
            Assert.AreEqual(expectedFrameIndex, frame3.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame3.TimeStampRelative);
            Assert.AreEqual(expectedPixelIndex3, frame3[0].Index);
            Assert.AreEqual(expectedColor3, frame3[0].Color);
        }

    }
}
