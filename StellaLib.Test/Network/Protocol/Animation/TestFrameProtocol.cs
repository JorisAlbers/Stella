using System;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
    [TestFixture]
    public class TestFrameWithoutDeltaProtocol
    {
        [Test]
        public void SerializeFrame_Frame_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameIndex = 9;
            FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex, 1, 3);
            frame[0] = new PixelInstruction(1,2,3);
            frame[1] = new PixelInstruction(4, 5, 6);
            frame[2] = new PixelInstruction(7, 8, 9);

            byte[] expectedBytes = new byte[sizeof(int) *3 + 3 + 3 + 3];
            int startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedBytes, startIndex); // Frame index
            BitConverter.GetBytes(1).CopyTo(expectedBytes, startIndex += 4); // Number of sections
            BitConverter.GetBytes(frame.Count).CopyTo(expectedBytes, startIndex += 4); // Number of pixel instructions
            // instruction 1
            expectedBytes[startIndex += 4] = 1;
            expectedBytes[startIndex += 1] = 2;
            expectedBytes[startIndex += 1] = 3;
            // instruction 2
            expectedBytes[startIndex += 1] = 4;
            expectedBytes[startIndex += 1] = 5;
            expectedBytes[startIndex += 1] = 6;
            // instruction 3
            expectedBytes[startIndex += 1] = 7;
            expectedBytes[startIndex += 1] = 8;
            expectedBytes[startIndex += 1] = 9;

            byte[][] packages = FrameProtocol.SerializeFrame(frame, maxPackageSize);

            Assert.AreEqual(1, packages.Length);
            CollectionAssert.AreEqual(expectedBytes, packages[0]);
        }

        [Test]
        public void SerializeFrame_FrameTooBigForASinglePackage_CreatesCorrectByteArray()
        {
            int maxPackageSize = 12 + 3 ; // 12 = size of frame section header, 3 = size of 1 pixel instruction.
            int frameIndex = 9;
            FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex, 1, 2);
            frame[0] = new PixelInstruction(1, 2, 3);
            frame[1] = new PixelInstruction(4, 5, 6);

            byte[] expectedPackage1 = new byte[12 + 3];
            int startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedPackage1, startIndex); // Frame index
            BitConverter.GetBytes(2).CopyTo(expectedPackage1, startIndex += 4); // Number of sections
            BitConverter.GetBytes(1).CopyTo(expectedPackage1, startIndex += 4); // Number of pixel instructions
            // instruction 1
            expectedPackage1[startIndex += 4] = 1;
            expectedPackage1[startIndex += 1] = 2;
            expectedPackage1[startIndex += 1] = 3;

            byte[] expectedPackage2 = new byte[12 + 3];
            startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedPackage2, startIndex); // Frame index
            BitConverter.GetBytes(1).CopyTo(expectedPackage2, startIndex += 4); // section index
            BitConverter.GetBytes(1).CopyTo(expectedPackage2, startIndex += 4); // Number of pixel instructions
            // instruction 2
            expectedPackage2[startIndex += 4] = 4;
            expectedPackage2[startIndex += 1] = 5;
            expectedPackage2[startIndex += 1] = 6;
            
            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(frame, maxPackageSize);
            
            // Assert
            Assert.AreEqual(2, packages.Length);
            CollectionAssert.AreEqual(expectedPackage1, packages[0]);
            CollectionAssert.AreEqual(expectedPackage2, packages[1]);
        }
    }
}