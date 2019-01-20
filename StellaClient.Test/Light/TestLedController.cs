using System.Drawing;
using Moq;
using NUnit.Framework;
using rpi_ws281x;
using StellaClient.Light;
using StellaLib.Animation;

namespace StellaClient.Test.Light
{
    [TestFixture]
    public class TestLedController
    {
        [Test]
        public void AddFrame_Frame_AddsFrameToTheQueue()
        {
            Frame frame = new Frame{ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
            var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);
            Assert.AreEqual(0, controller.FramesInBuffer);
            controller.AddFrame(frame);
            Assert.AreEqual(1, controller.FramesInBuffer);
        }
    }
}