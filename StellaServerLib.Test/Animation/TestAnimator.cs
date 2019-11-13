using System.Collections.Generic;
using System.Drawing;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

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

            int[] stripLengthPerPi = { 50 };

            List<Frame> frames = new List<Frame>
            {
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstructionWithDelta(0,1,2,3),
                }
            };

            var drawerMock = new Mock<IFrameProvider>();
            int index = -1;
            drawerMock.Setup(x => x.Current).Returns(()=> frames[index]);
            drawerMock.Setup(x => x.MoveNext()).Returns(true).Callback(() => index++);
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex , expectedPixelIndex)
            };

            var frameProviderCreatorMock = new Mock<IFrameProviderCreator>();
            StoryboardTransformationController transformationController;
            frameProviderCreatorMock.Setup(x => x.Create(It.IsAny<Storyboard>(),out transformationController)).Returns(drawerMock.Object);

            PlayList playList = new PlayList("test", new PlayListItem[]{new PlayListItem(new Storyboard(), 10)});
            Animator animator = new Animator(playList, frameProviderCreatorMock.Object, stripLengthPerPi, mask, new AnimationTransformationSettings(10,1,new float[3]));
            FrameWithoutDelta[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(1, framePerPi.Length);

            FrameWithoutDelta frame = framePerPi[0];
            Assert.AreEqual(expectedFrameIndex, frame.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame.TimeStampRelative);
            Assert.AreEqual(expectedColor, frame[expectedPixelIndex].ToColor());
           
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

            int[] stripLengthPerPi = {50, 50, 50};

            List<Frame> frames = new List<Frame>
            {
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstructionWithDelta(0,1,2,3),
                    new PixelInstructionWithDelta(1,4,5,6),
                    new PixelInstructionWithDelta(2,7,8,9),
                }
            };

            var drawerMock = new Mock<IFrameProvider>();
            int index = -1;
            drawerMock.Setup(x => x.Current).Returns(() => frames[index]);
            drawerMock.Setup(x => x.MoveNext()).Returns(true).Callback(() => index++);
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex1 , expectedPixelIndex1),
                new PiMaskItem(piIndex2 , expectedPixelIndex2),
                new PiMaskItem(piIndex3 , expectedPixelIndex3),
            };

            var frameProviderCreatorMock = new Mock<IFrameProviderCreator>();
            StoryboardTransformationController transformationController;
            frameProviderCreatorMock.Setup(x => x.Create(It.IsAny<Storyboard>() ,out transformationController)).Returns(drawerMock.Object);

            PlayList playList = new PlayList("test", new PlayListItem[] { new PlayListItem(new Storyboard(), 10) });
            Animator animator = new Animator(playList, frameProviderCreatorMock.Object, stripLengthPerPi, mask, new AnimationTransformationSettings(5, 1, new float[3]));
            FrameWithoutDelta[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(3,framePerPi.Length);
            // Pi1
            FrameWithoutDelta frame1 = framePerPi[0];
            Assert.AreEqual(50,frame1.Count);
            Assert.AreEqual(expectedFrameIndex,frame1.Index);
            Assert.AreEqual(expectedRelativeTimeStamp,frame1.TimeStampRelative);
            Assert.AreEqual(expectedColor1,frame1[expectedPixelIndex1].ToColor());
            // Pi1
            FrameWithoutDelta frame2 = framePerPi[1];
            Assert.AreEqual(50, frame2.Count);
            Assert.AreEqual(expectedFrameIndex, frame2.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame2.TimeStampRelative);
            Assert.AreEqual(expectedColor2, frame2[expectedPixelIndex2].ToColor());
            // Pi 3
            FrameWithoutDelta frame3 = framePerPi[2];
            Assert.AreEqual(50, frame3.Count);
            Assert.AreEqual(expectedFrameIndex, frame3.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame3.TimeStampRelative);
            Assert.AreEqual(expectedColor3, frame3[expectedPixelIndex3].ToColor());
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

            int[] stripLengthPerPi = { 50, 50, 50 };


            List<Frame> frames = new List<Frame>
            {
                new Frame(0,1),
                new Frame(1,2),
                new Frame(expectedFrameIndex,expectedRelativeTimeStamp)
                {
                    new PixelInstructionWithDelta(0,1,2,3),
                    new PixelInstructionWithDelta(1,4,5,6),
                    new PixelInstructionWithDelta(2,7,8,9),
                }
            };

            var drawerMock = new Mock<IFrameProvider>();
            int index = -1;
            drawerMock.Setup(x => x.Current).Returns(() => frames[index]);
            drawerMock.Setup(x => x.MoveNext()).Returns(true).Callback(() => index++);
            List<PiMaskItem> mask = new List<PiMaskItem>
            {
                new PiMaskItem(piIndex1 , expectedPixelIndex1),
                new PiMaskItem(piIndex2 , expectedPixelIndex2),
                new PiMaskItem(piIndex3 , expectedPixelIndex3),
            };

            var frameProviderCreatorMock = new Mock<IFrameProviderCreator>();
            StoryboardTransformationController transformationController;
            frameProviderCreatorMock.Setup(x => x.Create(It.IsAny<Storyboard>(), out transformationController)).Returns(drawerMock.Object);

            PlayList playList = new PlayList("test", new PlayListItem[] { new PlayListItem(new Storyboard(), 10) });
            Animator animator = new Animator(playList, frameProviderCreatorMock.Object, stripLengthPerPi, mask, new AnimationTransformationSettings(5, 1, new float[3]));
            // Flush first two frames
            animator.GetNextFramePerPi();
            animator.GetNextFramePerPi();

            // Assert
            FrameWithoutDelta[] framePerPi = animator.GetNextFramePerPi();

            Assert.AreEqual(3, framePerPi.Length);
            // Pi1
            FrameWithoutDelta frame1 = framePerPi[0];
            Assert.AreEqual(expectedFrameIndex, frame1.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame1.TimeStampRelative);
            Assert.AreEqual(expectedColor1, frame1[expectedPixelIndex1].ToColor());
            // Pi1
            FrameWithoutDelta frame2 = framePerPi[1];
            Assert.AreEqual(expectedFrameIndex, frame2.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame2.TimeStampRelative);
            Assert.AreEqual(expectedColor2, frame2[expectedPixelIndex2].ToColor());
            // Pi 3
            FrameWithoutDelta frame3 = framePerPi[2];
            Assert.AreEqual(expectedFrameIndex, frame3.Index);
            Assert.AreEqual(expectedRelativeTimeStamp, frame3.TimeStampRelative);
            Assert.AreEqual(expectedColor3, frame3[expectedPixelIndex3].ToColor());
        }

    }
}
