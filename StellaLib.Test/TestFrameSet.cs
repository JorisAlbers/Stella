using NUnit.Framework;
using StellaLib;

namespace StellaLib.Test
{
    [TestFixture]
    public class TestFrameSet
    {
        private Frame _frame1, _frame2, _frame3;

        [OneTimeSetUp]
        public void SetUp()
        {
            _frame1 = new Frame(){ pixelInstructions = new PixelInstruction[1]{ new PixelInstruction(){ Index = 1,   Red = 10, Green = 10, Blue = 10 }}};
            _frame2 = new Frame(){ pixelInstructions = new PixelInstruction[1]{ new PixelInstruction(){ Index = 10,  Red = 20, Green = 20, Blue = 20 }}};
            _frame3 = new Frame(){ pixelInstructions = new PixelInstruction[1]{ new PixelInstruction(){ Index = 100, Red = 30, Green = 30, Blue = 30 }}};
        }

        [Test]
        public void GetEnumerable_HasFrames_ReturnsTheCorrectFrames()
        {   
            Frame[] frames = new Frame[]
            {
                _frame1,
                _frame2,
                _frame3
            };
            FrameSet frameSet = new FrameSet(frames);
            
            // Assert on references
            int counter = 0;
            foreach (Frame frame in frameSet) 
            {
                Assert.AreEqual(frames[counter++], frame);
            }
        }
    }
}