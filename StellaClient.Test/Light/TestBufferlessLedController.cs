using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using rpi_ws281x;
using StellaClient.Light;
using StellaLib.Animation;

namespace StellaClient.Test.Light
{
    [TestFixture]
    public class TestBufferlessLedController
    {
        [Test]
        public void PrepareFrame_Frame_FrameGetsDrawnToStrip()
        {
            FrameWithoutDelta frame = new FrameWithoutDelta(0,100,1);
            frame[0] = new PixelInstructionWithoutDelta(Color.FromArgb(10, 20, 30));

            var mock = new Mock<ILEDStrip>();

            int index = -1;
            Color color;
            mock.Setup(x => x.SetLEDColor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Color>())).Callback<int, int, Color>((i, j, c) =>
            {
                index = j;
                color = c;
            });

            BufferlessLedController controller = new BufferlessLedController(mock.Object);
            controller.PrepareFrame(frame);
            Assert.AreEqual(0, index);
            Assert.AreEqual(Color.FromArgb(10, 20, 30), color);
        }

        [Test]
        public void Render__LedStripRenderGetsCalled()
        {
            FrameWithoutDelta frame = new FrameWithoutDelta(0, 100, 1);
            frame[0] = new PixelInstructionWithoutDelta(Color.FromArgb(10, 20, 30));
            var mock = new Mock<ILEDStrip>();
            mock.Setup(x => x.Render());

            BufferlessLedController controller = new BufferlessLedController(mock.Object);
            controller.PrepareFrame(frame);
            controller.Render();

            mock.Verify(x=>x.Render(),Times.Once);
        }
    }
}
