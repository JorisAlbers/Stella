using System;
using System.Collections;
using System.Collections.Generic;
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
            Frame frame = new Frame(0,100){ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
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
            controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
            controller.AddFrame(frame);
            Thread.Sleep(2000); // Async hack
            controller.Dispose();

            Assert.AreEqual(20,index);
            Assert.AreEqual(Color.FromArgb(10,20,30),color);
            
        }

        [Test]
        public void AddFrames_MultipleFrames_FramesGetsDrawnToStrip()
        {
            Frame frame1 = new Frame(0,100){ new PixelInstruction{ Index = 1, Color = Color.FromArgb(1,1,1)}};
            Frame frame2 = new Frame(1,100){ new PixelInstruction{ Index = 2, Color = Color.FromArgb(2,2,2)}};
            Frame frame3 = new Frame(2,100){ new PixelInstruction{ Index = 3, Color = Color.FromArgb(3,3,3)}};
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
            controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
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
        public void AddFrame_Frame_AddsFrameToThePendingQueue()
        {
            Frame frame = new Frame(0,100){ new PixelInstruction{ Index = 20, Color = Color.FromArgb(10,20,30)}};
            var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);
            Assert.AreEqual(0, controller.FramesInBuffer);
            controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
            controller.AddFrame(frame);
            Assert.AreEqual(1, controller.FramesInPendingBuffer);
        }

        [Test]
        public void PrepareNextFrameSet_FrameSetMetaData_FrameNeededGetsFired()
        {
           var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);

            int invokeCount = 0;
            int? lastFrameIndex = null;
            int count = 0;

            controller.FramesNeeded += (sender, args) =>
            {
                invokeCount++;
                lastFrameIndex = args.LastFrameIndex;
                count = args.Count;
            };

            controller.Run();
            controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));
            Thread.Sleep(100); // async hack. 
            
            Assert.AreEqual(1,invokeCount);
            Assert.AreEqual(null,lastFrameIndex);
            Assert.AreEqual(300,count); // FRAME_BUFFER_SIZE
        }

        
        [TestCase(102,200)] // The number of frames is +2 as on the first cycle the first two frames get drawn.
        [TestCase(52,250)]
        [TestCase(12,290)]
        public void AddFrame_BufferRunningLow__FrameNeededGetsFired(int numberOfFrames, int expectedCount)
        {
            List<Frame> frames = new List<Frame>();
            // The first frame has an TimeStampRelative of 0.
            frames.Add(new Frame(0, 0) { new PixelInstruction { Index = 20, Color = Color.FromArgb(10, 20, 30) }});

            int waitMS = int.MaxValue; // Really high so only the first frame will be on display during the test.
            for (int i = 1; i < numberOfFrames; i++)
            {
               frames.Add(new Frame(i, waitMS) { new PixelInstruction { Index = 20, Color = Color.FromArgb(10, 20, 30) }});
            }

            var mock = new Mock<ILEDStrip>();
            LedController controller = new LedController(mock.Object);

            int invokeCount = 0;
            int? lastFrameIndex = null;
            int count = 0;
            controller.PrepareNextFrameSet(new FrameSetMetadata(DateTime.Now));

            controller.FramesNeeded += (sender, args) =>
            {
                invokeCount++;
                lastFrameIndex = args.LastFrameIndex;
                count = args.Count;
            };

            controller.AddFrames(frames);
            controller.Run();

            Thread.Sleep(1500); // async hack. 


            Assert.AreEqual(1, invokeCount, "The invoke count was incorrect");
            Assert.AreEqual(numberOfFrames -1, lastFrameIndex, "The frame index was incorrect" );
            Assert.AreEqual(expectedCount, count, "The count was incorrect"); // FRAME_BUFFER_SIZE - frames in buffer. The LedController dequeues the first 2 frames immediately.

        }
    }
}