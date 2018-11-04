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
            _frame1 = new Frame(){ new PixelInstruction(){ Index = 1,   Color = new Color(){ Red = 10, Green = 10, Blue = 10 }}};
            _frame2 = new Frame(){ new PixelInstruction(){ Index = 10,  Color = new Color(){ Red = 20, Green = 20, Blue = 20 }}};
            _frame3 = new Frame(){ new PixelInstruction(){ Index = 100, Color = new Color(){ Red = 30, Green = 30, Blue = 30 }}};
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

            // Assert
            int counter = 0;
            foreach (Frame frame in frameSet) 
            {
                Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[counter++], frame[0].Index);
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

            // Assert
            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[0],frameSet[0][0].Index);
            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[1],frameSet[1][0].Index);
            Assert.AreEqual(expectedIndexesOfFirstPixelIndexOfEachFrame[2],frameSet[2][0].Index);
        }
    }
}