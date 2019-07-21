using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Test.Animation.FrameProvider
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
                    new PixelInstruction(0,Color.Red),
                },
                new Frame(0,0)
                {
                    new PixelInstruction(1,Color.Blue),
                }
            };

            var drawerMock = new Mock<IDrawer>();
            drawerMock.Setup(x => x.GetEnumerator()).Returns(drawerFrames.GetEnumerator());
            AnimationTransformation animationTransformation = new AnimationTransformation(100);
            StellaServerLib.Animation.FrameProvider.FrameProvider frameProvider =  new StellaServerLib.Animation.FrameProvider.FrameProvider(drawerMock.Object, animationTransformation);

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
    }
}
