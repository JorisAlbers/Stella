using System.Drawing;
using Moq;
using NUnit.Framework;
using rpi_ws281x;
using StellaClientLib.Light;
using StellaLib.Animation;

namespace StellaClient.Test.Light
{
    [TestFixture]
    public class TestLedController
    {
        [Test]
        public void PrepareFrame_Frame_FrameGetsDrawnToStrip()
        {
            FrameWithoutDelta frame = new FrameWithoutDelta(0,100,1);
            frame[0] = new PixelInstruction(10, 20, 30);

            var mock = new Mock<ILEDStrip>();

            int index = -1;
            Color color;
            mock.Setup(x => x.SetLEDColor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Color>())).Callback<int, int, Color>((i, j, c) =>
            {
                index = j;
                color = c;
            });

            LedController controller = new LedController(mock.Object,50);
            controller.RenderFrame(frame);
            Assert.AreEqual(0, index);
            Assert.AreEqual(Color.FromArgb(10, 20, 30), color);
        }

        [Test]
        public void Render__LedStripRenderGetsCalled()
        {
            FrameWithoutDelta frame = new FrameWithoutDelta(0, 100, 1);
            frame[0] = new PixelInstruction(10, 20, 30);
            var mock = new Mock<ILEDStrip>();
            mock.Setup(x => x.Render());

            LedController controller = new LedController(mock.Object,50);
            controller.RenderFrame(frame);

            mock.Verify(x=>x.Render(),Times.Once);
        }
    }
}
