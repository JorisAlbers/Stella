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
            Frame frame = new Frame(100){ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
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
        public void AddFrames_MultipleFrames_FramesGetsDrawnToStrip()
        {
            Frame frame1 = new Frame(100){ new PixelInstruction{ Index = 1, Color = Color.FromArgb(1,1,1)}};
            Frame frame2 = new Frame(100){ new PixelInstruction{ Index = 2, Color = Color.FromArgb(2,2,2)}};
            Frame frame3 = new Frame(100){ new PixelInstruction{ Index = 3, Color = Color.FromArgb(3,3,3)}};
            Frame[] frames = new Frame[]{frame1,frame2,frame3};

            var mock = new Mock<ILEDStrip>();

            int index1 = -1;
            int index2 = -1;
            int index3 = -1;
            Color color1;
            Color color2;
            Color color3;

            int callbackCount = 0;
            mock.Setup(x=> x.SetLEDColor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Color>())).Callback<int,int,Color>((i,j,c) =>
            {
                if(callbackCount == 0)
                {
                    index1 = j;
                    color1 = c;
                }
                else if(callbackCount == 1)
                {
                    index2 = j;
                    color2 = c;
                }
                else
                {
                    index3 = j;
                    color3 = c;
                }
                callbackCount++;
            });

            LedController controller = new LedController(mock.Object);
            controller.Run();
            controller.AddFrames(frames);
            Thread.Sleep(2000); // Async hack
            controller.Dispose();

            Assert.AreEqual(1,index1);
            Assert.AreEqual(2,index2);
            Assert.AreEqual(3,index3);
            Assert.AreEqual(Color.FromArgb(1,1,1),color1);
            Assert.AreEqual(Color.FromArgb(2,2,2),color2);
            Assert.AreEqual(Color.FromArgb(3,3,3),color3);
            
        }

        [Test]
        public void AddFrame_Frame_AddsFrameToTheQueue()
        {
            Frame frame = new Frame(100){ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
            var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);
            Assert.AreEqual(0, controller.FramesInBuffer);
            controller.AddFrame(frame);
            Assert.AreEqual(1, controller.FramesInBuffer);
        }

        [Test]
        public void ClearFrameBuffer_BufferWithFrames_ClearsTheFrameBuffer()
        {
            Frame frame = new Frame(100){ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
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