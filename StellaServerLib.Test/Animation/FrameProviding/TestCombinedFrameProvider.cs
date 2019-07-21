﻿using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.FrameProviding;

namespace StellaServerLib.Test.Animation.FrameProviding
{
    [TestFixture]
    class TestCombinedFrameProvider
    {
        [Test]
        public void IEnumerable_TwoFrameProvidersAtDelayFromEachOther_CorrectlyIteratesFrames()
        {
            List<Frame> frames1 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(0, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(2, 1, 2, 3)
                },
                new Frame(2, 200)
                {
                    new PixelInstruction(3, 1, 2, 3)
                }
            };

            List<Frame> frames2 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(7, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(8, 1, 2, 3)
                },
                new Frame(2, 100)
                {
                    new PixelInstruction(9, 1, 2, 3)
                }
            };

            // EXPECTED
            Frame expectedFrame1 = new Frame(0, 0) { frames1[0][0] };
            Frame expectedFrame2 = new Frame(1, 50) { frames2[0][0] };
            Frame expectedFrame3 = new Frame(2, 100) { frames1[1][0] };
            Frame expectedFrame4 = new Frame(3, 150) { frames2[1][0] };


            var mockDrawer1 = new Mock<IFrameProvider>();
            mockDrawer1.Setup(x => x.GetEnumerator()).Returns(frames1.GetEnumerator());

            var mockDrawer2 = new Mock<IFrameProvider>();
            mockDrawer2.Setup(x => x.GetEnumerator()).Returns(frames2.GetEnumerator());

            int start1 = 0;
            int start2 = 50; // 50ms to make sure they get out of frame
            CombinedFrameProvider sectionDrawer = new CombinedFrameProvider(new IFrameProvider[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 });

            List<Frame> receivedFrames = sectionDrawer.Take(4).ToList();

            Assert.AreEqual(expectedFrame1, receivedFrames[0]);
            Assert.AreEqual(expectedFrame2, receivedFrames[1]);
            Assert.AreEqual(expectedFrame3, receivedFrames[2]);
            Assert.AreEqual(expectedFrame4, receivedFrames[3]);
        }

        [Test]
        public void IEnumerable_TwoDrawersInFrame_CorrectlyIteratesFrames()
        {
            List<Frame> frames1 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(0, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(2, 1, 2, 3)
                },
                new Frame(2, 200)
                {
                    new PixelInstruction(3, 1, 2, 3)
                }
            };

            List<Frame> frames2 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstruction(7, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstruction(8, 1, 2, 3)
                },
                new Frame(2, 100)
                {
                    new PixelInstruction(9, 1, 2, 3)
                }
            };

            // EXPECTED
            Frame expectedFrame1 = new Frame(0, 0) { frames1[0][0], frames2[0][0] };
            Frame expectedFrame2 = new Frame(1, 100) { frames1[1][0], frames2[1][0] };

            var mockDrawer1 = new Mock<IFrameProvider>();
            mockDrawer1.Setup(x => x.GetEnumerator()).Returns(frames1.GetEnumerator());

            var mockDrawer2 = new Mock<IFrameProvider>();
            mockDrawer2.Setup(x => x.GetEnumerator()).Returns(frames2.GetEnumerator());

            int start1 = 0;
            int start2 = 0; // Both are in frame
            CombinedFrameProvider sectionDrawer = new CombinedFrameProvider(new IFrameProvider[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 });

            List<Frame> receivedFrames = sectionDrawer.Take(2).ToList();

            Assert.AreEqual(expectedFrame1, receivedFrames[0]);
            Assert.AreEqual(expectedFrame2, receivedFrames[1]);
        }
    }
}