using System.Drawing;
using NUnit.Framework;
using StellaLib.Animation;

namespace StellaLib.Test.Animation
{
    [TestFixture]
    public class TestFrameSet
    {
        private Frame _frame1, _frame2, _frame3;

        [OneTimeSetUp]
        public void SetUp()
        {
            _frame1 = new Frame(0,1){ new PixelInstruction(){ Index = 1,   Color = Color.FromArgb(10,10,10)}};
            _frame2 = new Frame(1,2){ new PixelInstruction(){ Index = 10,  Color = Color.FromArgb(20,20,20)}};
            _frame3 = new Frame(2,3){ new PixelInstruction(){ Index = 100, Color = Color.FromArgb(30,30,30)}};
        }

        [Test]
        public void GetEnumerable_HasFrames_ReturnsTheCorrectFrames()
        {   
            //Setup
            FrameSet frameSet = new FrameSet();
            frameSet.Add(_frame1);
            frameSet.Add(_frame2);
            frameSet.Add(_frame3);
            
            // Expected on first pixel instruction of each frame.
            int[] expectedIndexesOfFirstPixelIndexOfEachFrame = new int[]
            {
                1,
                10,
                100
            };

            // Expected wait ms
            int[] expectedWaitMSOfEachFrame = new int[]
            {
                1,
                2,
                3
            };

            // Assert
            int counter = 0;
            foreach (Frame frame in frameSet) 
            {
                Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[counter], frame[0].Index);
                Assert.AreEqual(expectedWaitMSOfEachFrame[counter], frame.WaitMS);
                counter++;
            }
        }

        [Test]
        public void Index_HasFrames_ReturnsTheCorrectFrames()
        {   
            //Setup
            FrameSet frameSet = new FrameSet();
            frameSet.Add(_frame1);
            frameSet.Add(_frame2);
            frameSet.Add(_frame3);
            
            // Expected on first pixel instruction of each frame.
            int[] expectedIndexesOfFirstPixelIndexOfEachFrame = new int[]
            {
                1,
                10,
                100
            };
            // Expected wait ms
            int[] expectedWaitMSOfEachFrame = new int[]
            {
                1,
                2,
                3
            };

            // Assert
            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[0],frameSet[0][0].Index);
            Assert.AreEqual(expectedWaitMSOfEachFrame[0],frameSet[0].WaitMS);

            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[1],frameSet[1][0].Index);
            Assert.AreEqual(expectedWaitMSOfEachFrame[1],frameSet[1].WaitMS);

            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[2],frameSet[2][0].Index);
            Assert.AreEqual(expectedWaitMSOfEachFrame[2],frameSet[2].WaitMS);
        }
    }
}