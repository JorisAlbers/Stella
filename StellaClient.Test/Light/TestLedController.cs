using System.Drawing;
using System.Threading;
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
        public void AddFrame_Frame_FrameGetsDrawnToStrip()
        {
            Frame frame = new Frame{ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
            var mock = new Mock<ILEDStrip>();

            int index = -1;
            Color color;
            mock.Setup(x=> x.SetLEDColor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Color>())).Callback<int,int,Color>((i,j,c) =>
            {
                index = j;
                color = c;
            });

            LedController controller = new LedController(mock.Object);
            controller.Run();
            controller.AddFrame(frame);
            Thread.Sleep(2000); // Async hack
            controller.Dispose();

            Assert.AreEqual(20,index);
            Assert.AreEqual(Color.FromArgb(10,20,30),color);
            
        }

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

        [Test]
        public void ClearFrameBuffer_BufferWithFrames_ClearsTheFrameBuffer()
        {
            Frame frame = new Frame{ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
            var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);
            controller.AddFrame(frame);
            controller.AddFrame(frame);
            controller.AddFrame(frame);
            Assert.AreEqual(3, controller.FramesInBuffer);
            controller.ClearFrameBuffer();
            Assert.AreEqual(0, controller.FramesInBuffer);
        }
        
    }
}