using System;
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
            int index = -1;
            drawerMock.Setup(x => x.Current).Returns(() => drawerFrames[index]);
            drawerMock.Setup(x => x.MoveNext()).Returns(true).Callback(() => index++);


            StoryboardTransformationController transformationController = new StoryboardTransformationController(new AnimationTransformationSettings(0,0,new float[3]),new AnimationTransformationSettings[]
            {
                new AnimationTransformationSettings(100,0,new float[3]), 
            } );
            FrameProvider frameProvider =  new FrameProvider(drawerMock.Object, transformationController,1);

            // Frame 1
            frameProvider.MoveNext();
            Assert.AreEqual(0, frameProvider.Current.Index);
            Assert.AreEqual(0, frameProvider.Current.TimeStampRelative);
            Assert.AreEqual(1, frameProvider.Current.Count);
            // Frame 2
            frameProvider.MoveNext();
            Assert.AreEqual(1,   frameProvider.Current.Index);
            Assert.AreEqual(100, frameProvider.Current.TimeStampRelative);
            Assert.AreEqual(1,   frameProvider.Current.Count);

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
            int index = -1;
            drawerMock.Setup(x => x.Current).Returns(()=>drawerFrames[index]);
            drawerMock.Setup(x => x.MoveNext()).Returns(true).Callback(() => index++);
            StoryboardTransformationController transformationController = new StoryboardTransformationController(new AnimationTransformationSettings(0, 0, new float[3]), new AnimationTransformationSettings[]
            {
                new AnimationTransformationSettings(timeUnitsPerFrame, 0 , new float[3]),
            });
            FrameProvider frameProvider = new FrameProvider(drawerMock.Object, transformationController, timeUnitMs);

            frameProvider.MoveNext();
            // Frame 1. TimeStampRelative is always 0.
            Assert.AreEqual(0, frameProvider.Current.Index);
            Assert.AreEqual(0, frameProvider.Current.TimeStampRelative);
            Assert.AreEqual(1, frameProvider.Current.Count);
            // Frame 2
            frameProvider.MoveNext();
            Assert.AreEqual(1, frameProvider.Current.Index);
            Assert.AreEqual(expectedTimeStampRelative, frameProvider.Current.TimeStampRelative);
            Assert.AreEqual(1, frameProvider.Current.Count);

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
            int index1 = -1;
            mockDrawer1.Setup(x => x.Current).Returns(()=>frames1[index1]);
            mockDrawer1.Setup(x => x.MoveNext()).Returns(true).Callback(() => index1++);


            var mockDrawer2 = new Mock<IDrawer>();
            int index2 = -1;
            mockDrawer2.Setup(x => x.Current).Returns(()=>frames2[index2]);
            mockDrawer2.Setup(x => x.MoveNext()).Returns(true).Callback(() => index2++);




            int start1 = 0;
            int start2 = 50; // 50ms to make sure they get out of frame
            StoryboardTransformationController transformationController = new StoryboardTransformationController(new AnimationTransformationSettings(0, 0, new float[3]), new AnimationTransformationSettings[]
            {
                new AnimationTransformationSettings(100,0,new float[3]),
                new AnimationTransformationSettings(100,0,new float[3]),
            });
            FrameProvider frameProvider = new FrameProvider(new IDrawer[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 }, transformationController,1);


            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame1, frameProvider.Current);
            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame2, frameProvider.Current);
            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame3, frameProvider.Current);
            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame4, frameProvider.Current);
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
            int index1 = -1;
            mockDrawer1.Setup(x => x.Current).Returns(()=>frames1[index1]);
            mockDrawer1.Setup(x => x.MoveNext()).Returns(true).Callback(() => index1++);

            var mockDrawer2 = new Mock<IDrawer>();
            int index2 = -1;
            mockDrawer2.Setup(x => x.Current).Returns(()=>frames2[index2]);
            mockDrawer2.Setup(x => x.MoveNext()).Returns(true).Callback(() => index2++);


            int start1 = 0;
            int start2 = 0; // Both are in frame
            StoryboardTransformationController transformationController = new StoryboardTransformationController(new AnimationTransformationSettings(0, 0, new float[3]), new AnimationTransformationSettings[]
            {
                new AnimationTransformationSettings(100,0,new float[3]),
                new AnimationTransformationSettings(100,0,new float[3]),
            });

            FrameProvider frameProvider = new FrameProvider(new IDrawer[] { mockDrawer1.Object, mockDrawer2.Object }, new int[] { start1, start2 }, transformationController,1);

            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame1, frameProvider.Current);
            frameProvider.MoveNext();
            Assert.AreEqual(expectedFrame2, frameProvider.Current);
        }
    }
}
