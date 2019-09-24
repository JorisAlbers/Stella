using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Test.Animation.FrameProviding
{
    [TestFixture]
    public class TestFrameProvider
    {
        [Test]
        public void GetEnumerator_CorrectlyCreatesFrames()
        {
            List<Frame> drawerFrames = new List<Frame>
            {
                new Frame(0,0)
                {
                    new PixelInstructionWithDelta(0,255,0,0),
                },
                new Frame(0,0)
                {
                    new PixelInstructionWithDelta(1,0,0,255),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(drawerFrames.GetEnumerator());
            TransformationController transformationController = new TransformationController(new TransformationSettings(0,0,new float[3]),new TransformationSettings[]
            {
                new TransformationSettings(100,0,new float[3]), 
            } );
            FrameProvider frameProvider =  new FrameProvider(drawerMock.Object, transformationController,1);

            Frame[] frames = frameProvider.Take(2).ToArray();
            // Frame 1
            Assert.AreEqual(0,frames[0].Index);
            Assert.AreEqual(0,frames[0].TimeStampRelative);
            Assert.AreEqual(1,frames[0].Count);
            // Frame 2
            Assert.AreEqual(1,frames[1].Index);
            Assert.AreEqual(100,frames[1].TimeStampRelative);
            Assert.AreEqual(1,frames[1].Count);

        }

        /// <summary>
        /// Tests that a different timeUnitMs also increases the timeStampRelative of a frame
        /// </summary>
        [Test]
        public void GetEnumerator_TimeUnitMsOf5_CorrectlyCreatesFrames()
        {
            int timeUnitMs = 10;
            int timeUnitsPerFrame = 100;
            int expectedTimeStampRelative = timeUnitMs * timeUnitsPerFrame;
            List<Frame> drawerFrames = new List<Frame>
            {
                new Frame(0,0)
                {
                    new PixelInstructionWithDelta(0,255,0,0),
                },
                new Frame(0,0)
                {
                    new PixelInstructionWithDelta(1,0,0,255),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(drawerFrames.GetEnumerator());
            TransformationController transformationController = new TransformationController(new TransformationSettings(0, 0, new float[3]), new TransformationSettings[]
            {
                new TransformationSettings(timeUnitsPerFrame, 0 , new float[3]),
            });
            FrameProvider frameProvider = new FrameProvider(drawerMock.Object, transformationController, timeUnitMs);

            Frame[] frames = frameProvider.Take(2).ToArray();
            // Frame 1. TimeStampRelative is always 0.
            Assert.AreEqual(0, frames[0].Index);
            Assert.AreEqual(0, frames[0].TimeStampRelative);
            Assert.AreEqual(1, frames[0].Count);
            // Frame 2
            Assert.AreEqual(1, frames[1].Index);
            Assert.AreEqual(expectedTimeStampRelative, frames[1].TimeStampRelative);
            Assert.AreEqual(1, frames[1].Count);

        }

        [Test]
        public void IEnumerable_TwoFrameProvidersAtDelayFromEachOther_CorrectlyIteratesFrames()
        {
            List<Frame> frames1 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstructionWithDelta(0, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstructionWithDelta(2, 1, 2, 3)
                },
                new Frame(2, 200)
                {
                    new PixelInstructionWithDelta(3, 1, 2, 3)
                }
            };

            List<Frame> frames2 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstructionWithDelta(7, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstructionWithDelta(8, 1, 2, 3)
                },
                new Frame(2, 100)
                {
                    new PixelInstructionWithDelta(9, 1, 2, 3)
                }
            };

            // EXPECTED
            Frame expectedFrame1 = new Frame(0, 0) { frames1[0][0] };
            Frame expectedFrame2 = new Frame(1, 50) { frames2[0][0] };
            Frame expectedFrame3 = new Frame(2, 100) { frames1[1][0] };
            Frame expectedFrame4 = new Frame(3, 150) { frames2[1][0] };


            var mockDrawer1 = new Mock<IDrawer>();
            mockDrawer1.Setup(x => x.GetEnumerator()).Returns(frames1.GetEnumerator());

            var mockDrawer2 = new Mock<IDrawer>();
            mockDrawer2.Setup(x => x.GetEnumerator()).Returns(frames2.GetEnumerator());

            int start1 = 0;
            int start2 = 50; // 50ms to make sure they get out of frame
            TransformationController transformationController = new TransformationController(new TransformationSettings(0, 0, new float[3]), new TransformationSettings[]
            {
                new TransformationSettings(100,0,new float[3]),
                new TransformationSettings(100,0,new float[3]),
            });
            FrameProvider sectionDrawer = new FrameProvider(new IDrawer[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 }, transformationController,1);


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
                    new PixelInstructionWithDelta(0, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstructionWithDelta(2, 1, 2, 3)
                },
                new Frame(2, 200)
                {
                    new PixelInstructionWithDelta(3, 1, 2, 3)
                }
            };

            List<Frame> frames2 = new List<Frame>
            {
                new Frame(0, 0)
                {
                    new PixelInstructionWithDelta(7, 1, 2, 3)
                },
                new Frame(1, 100)
                {
                    new PixelInstructionWithDelta(8, 1, 2, 3)
                },
                new Frame(2, 100)
                {
                    new PixelInstructionWithDelta(9, 1, 2, 3)
                }
            };

            // EXPECTED
            Frame expectedFrame1 = new Frame(0, 0) { frames1[0][0], frames2[0][0] };
            Frame expectedFrame2 = new Frame(1, 100) { frames1[1][0], frames2[1][0] };

            var mockDrawer1 = new Mock<IDrawer>();
            mockDrawer1.Setup(x => x.GetEnumerator()).Returns(frames1.GetEnumerator());

            var mockDrawer2 = new Mock<IDrawer>();
            mockDrawer2.Setup(x => x.GetEnumerator()).Returns(frames2.GetEnumerator());

            int start1 = 0;
            int start2 = 0; // Both are in frame
            TransformationController transformationController = new TransformationController(new TransformationSettings(0, 0, new float[3]), new TransformationSettings[]
            {
                new TransformationSettings(100,0,new float[3]),
                new TransformationSettings(100,0,new float[3]),
            });

            FrameProvider sectionDrawer = new FrameProvider(new IDrawer[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 }, transformationController,1);

            List<Frame> receivedFrames = sectionDrawer.Take(2).ToList();

            Assert.AreEqual(expectedFrame1, receivedFrames[0]);
            Assert.AreEqual(expectedFrame2, receivedFrames[1]);
        }
    }
}
