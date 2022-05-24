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

            byte[] expectedBytes = new byte[sizeof(int)  + sizeof(int) + sizeof(bool) + 3 + 3 + 3];
            int startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedBytes, startIndex); // Frame index
            BitConverter.GetBytes(frame.Count).CopyTo(expectedBytes, startIndex += 4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(expectedBytes, startIndex += 4); // Has FrameSections
            // instruction 1
            expectedBytes[startIndex += 1] = 1;
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
            Assert.AreEqual(expectedBytes, packages[0]);
        }

        [Test]
        public void SerializeFrame_FrameTooBigForASinglePackage_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameIndex = 9;
            FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex, 1,900);
            for (int i = 0; i < 900; i++)
            {
                frame[i] = new PixelInstruction(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(frame, maxPackageSize);

            // Deserialize
            FrameProtocol frameProtocol = new FrameProtocol();
            FrameWithoutDelta deserializedFrame = null;
            foreach (byte[] package in packages)
            {
                if(frameProtocol.TryDeserialize(package, out FrameWithoutDelta returnFrame))
                {
                    deserializedFrame = returnFrame;
                }
            }

            // Assert
            Assert.AreEqual(3, packages.Length);
            Assert.IsNotNull(deserializedFrame);
            CollectionAssert.AreEqual(frame.Items, deserializedFrame.Items);

        }

        [Test]
        public void TryDeserialize_frameWithNoFrameSets_DeserializesCorrectly()
        {
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(1, 1, 3);
            expectedFrame[0] = new PixelInstruction (1, 2, 3) ;
            expectedFrame[1] = new PixelInstruction (4, 5, 6) ;
            expectedFrame[2] = new PixelInstruction (7, 8, 9) ;

            byte[] bytes = new byte[sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + PixelInstructionProtocol.BYTES_NEEDED * 3];
            int startIndex = 0;
            BitConverter.GetBytes(1).CopyTo(bytes, startIndex); // Frame index
            BitConverter.GetBytes(expectedFrame.Count).CopyTo(bytes, startIndex += 4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(bytes, startIndex += 4); // Has FrameSections
            // instruction 1
            bytes[startIndex += 1] = 1;
            bytes[startIndex += 1] = 2;
            bytes[startIndex += 1] = 3;
            // instruction 2
            bytes[startIndex += 1] = 4;
            bytes[startIndex += 1] = 5;
            bytes[startIndex += 1] = 6;
            // instruction 3
            bytes[startIndex += 1] = 7;
            bytes[startIndex += 1] = 8;
            bytes[startIndex += 1] = 9;

            FrameProtocol protocol = new FrameProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(true, protocol.TryDeserialize(bytes, out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSets_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, 1,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstruction(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);

            FrameProtocol protocol = new FrameProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(false, protocol.TryDeserialize(packages[0], out frame));
            Assert.AreEqual(false, protocol.TryDeserialize(packages[1], out frame));
            Assert.AreEqual(true, protocol.TryDeserialize(packages[2], out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSetsOutOfOrder_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, 1,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstruction(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);

            FrameProtocol protocol = new FrameProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(false, protocol.TryDeserialize(packages[0], out frame));
            Assert.AreEqual(false, protocol.TryDeserialize(packages[2], out frame));
            Assert.AreEqual(true, protocol.TryDeserialize(packages[1], out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEquivalent(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void GetFrameIndex_FrameAndFrameSection_GetsCorrectIndex()
        {

            int maxPackageSize = 1016;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, 1,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstruction(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);
            Assert.AreEqual(frameIndex, FrameProtocol.GetFrameIndex(packages[0]));
            Assert.AreEqual(frameIndex, FrameProtocol.GetFrameIndex(packages[1]));
            Assert.AreEqual(frameIndex, FrameProtocol.GetFrameIndex(packages[2]));
        }
    }
}