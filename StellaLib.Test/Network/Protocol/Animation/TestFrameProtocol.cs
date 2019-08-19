using System;
using System.Linq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
    [TestFixture]
    public class TestFrameProtocol
    {
        [Test]
        public void SerializeFrame_Frame_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            Frame frame = new Frame(frameIndex,frameWaitMS)
            { 
                new PixelInstruction(1,   1,2,3),
                new PixelInstruction(2,   4,5,6),
                new PixelInstruction(10,  7,8,9)
            };
            byte[] expectedBytes = new byte[sizeof(int)+ sizeof(int) +sizeof(int) + sizeof(bool) + sizeof(int)+3 + sizeof(int)+3 + sizeof(int)+3];
            int startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedBytes,startIndex); // Frame index
            BitConverter.GetBytes(frameWaitMS).CopyTo(expectedBytes,startIndex+=4); 
            BitConverter.GetBytes(frame.Count).CopyTo(expectedBytes,startIndex+=4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(expectedBytes,startIndex+=4); // Has FrameSections
            // instruction 1
            BitConverter.GetBytes(1).CopyTo(expectedBytes,startIndex+=1);
            expectedBytes[startIndex+=4] = (byte)1;
            expectedBytes[startIndex+=1] = (byte)2;
            expectedBytes[startIndex+=1] = (byte)3;
            // instruction 2
            BitConverter.GetBytes(2).CopyTo(expectedBytes,startIndex+=1);
            expectedBytes[startIndex+=4] = (byte)4;
            expectedBytes[startIndex+=1] = (byte)5;
            expectedBytes[startIndex+=1] = (byte)6;
            // instruction 3
            BitConverter.GetBytes(10).CopyTo(expectedBytes,startIndex+=1);
            expectedBytes[startIndex+=4] = (byte)7;
            expectedBytes[startIndex+=1] = (byte)8;
            expectedBytes[startIndex+=1] = (byte)9;

            byte[][] packages = FrameProtocol.SerializeFrame(frame, maxPackageSize);

            Assert.AreEqual(1, packages.Length);
            Assert.AreEqual(expectedBytes, packages[0]);
        }

         [Test]
        public void SerializeFrame_FrameTooBigForASinglePackage_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            Frame frame = new Frame(frameIndex,frameWaitMS);
            for (int i = 0; i < 300; i++)
            {
                frame.Add(new PixelInstruction(i,1,2,3));
            }

            //Expected
            // First package, check only header bytes
            byte[] expectedHeaderBytes1 = new byte[sizeof(int)+ sizeof(int) +sizeof(int) + sizeof(bool) + FrameSectionProtocol.HEADER_BYTES_NEEDED];
            BitConverter.GetBytes(frameIndex).CopyTo(expectedHeaderBytes1,0); // Frame index
            BitConverter.GetBytes(frameWaitMS).CopyTo(expectedHeaderBytes1,4); // Timestamp, relative
            BitConverter.GetBytes(3).CopyTo(expectedHeaderBytes1,8); // Number of FrameSections
            BitConverter.GetBytes(true).CopyTo(expectedHeaderBytes1,12); // Has FrameSections
            BitConverter.GetBytes(frameIndex).CopyTo(expectedHeaderBytes1,13); // frame sequence index
            BitConverter.GetBytes(0).CopyTo(expectedHeaderBytes1,17); //package index
            BitConverter.GetBytes(141).CopyTo(expectedHeaderBytes1,21); // number of pixelinstruction in the FrameSectionPackage, for the first package, this is 141

            // Second package, check only header bytes
            byte[] expectedHeaderBytes2 = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED];
            BitConverter.GetBytes(frameIndex).CopyTo(expectedHeaderBytes2,0); // frame sequence index
            BitConverter.GetBytes(1).CopyTo(expectedHeaderBytes2,4); //package index
            BitConverter.GetBytes(143).CopyTo(expectedHeaderBytes2,8); // number of pixelinstruction in the FrameSectionPackage, for subsequent packages this is 143

            // Third package, check header + first pixel instruction
            byte[] expectedHeaderBytes3 = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED + PixelInstructionProtocol.BYTES_NEEDED];
            BitConverter.GetBytes(frameIndex).CopyTo(expectedHeaderBytes3,0); // frame sequence index
            BitConverter.GetBytes(2).CopyTo(expectedHeaderBytes3,4); //package index
            BitConverter.GetBytes(16).CopyTo(expectedHeaderBytes3,8); // number of pixelinstruction in the FrameSectionPackage, for subsequent packages this is 143, so there are 16 left
            PixelInstruction instructionAtStartOfFrameSet3 = frame[284];
            PixelInstructionProtocol.Serialize(instructionAtStartOfFrameSet3,expectedHeaderBytes3,12);
            
            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(frame,maxPackageSize);

            // Assert
            Assert.AreEqual(3,packages.Length);
            Assert.AreEqual(expectedHeaderBytes1,packages[0].Take(expectedHeaderBytes1.Length).ToArray(), "package 1 header incorrect");
            Assert.AreEqual(expectedHeaderBytes2,packages[1].Take(expectedHeaderBytes2.Length).ToArray(), "package 2 header incorrect");
            Assert.AreEqual(expectedHeaderBytes3,packages[2].Take(expectedHeaderBytes3.Length).ToArray(), "package 3 header or first instruction incorrect");
           
        }

        [Test]
        public void TryDeserialize_frameWithNoFrameSets_DeserializesCorrectly()
        {
            int frameWaitMS = 100;
            Frame expectedFrame = new Frame(1,frameWaitMS)
            { 
                new PixelInstruction(1,   1,2,3),
                new PixelInstruction(2,   4,5,6),
                new PixelInstruction(10,  7,8,9)
            };
            byte[] bytes = new byte[sizeof(int)+ sizeof(int) +sizeof(int) + sizeof(bool) + PixelInstructionProtocol.BYTES_NEEDED * 3];
            int startIndex = 0;
            BitConverter.GetBytes(1).CopyTo(bytes,startIndex); // Frame index
            BitConverter.GetBytes(frameWaitMS).CopyTo(bytes,startIndex+=4); // Timestamp (relative)
            BitConverter.GetBytes(expectedFrame.Count).CopyTo(bytes,startIndex+=4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(bytes,startIndex+=4); // Has FrameSections
            // instruction 1
            BitConverter.GetBytes(1).CopyTo(bytes,startIndex+=1);
            bytes[startIndex+=4] = (byte)1;
            bytes[startIndex+=1] = (byte)2;
            bytes[startIndex+=1] = (byte)3;
            // instruction 2
            BitConverter.GetBytes(2).CopyTo(bytes,startIndex+=1);
            bytes[startIndex+=4] = (byte)4;
            bytes[startIndex+=1] = (byte)5;
            bytes[startIndex+=1] = (byte)6;
            // instruction 3
            BitConverter.GetBytes(10).CopyTo(bytes,startIndex+=1);
            bytes[startIndex+=4] = (byte)7;
            bytes[startIndex+=1] = (byte)8;
            bytes[startIndex+=1] = (byte)9;

            FrameProtocol protocol = new FrameProtocol();
            Frame frame;
            Assert.AreEqual(true,protocol.TryDeserialize(bytes,out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame,frame);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSets_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            Frame expectedFrame = new Frame(frameIndex,frameWaitMS);
            for (int i = 0; i < 300; i++)
            {
                expectedFrame.Add(new PixelInstruction(i,1,2,3));
            }      
            
            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame,maxPackageSize);

            Assert.AreEqual(3,packages.Length);

            FrameProtocol protocol = new FrameProtocol();
            Frame frame;
            Assert.AreEqual(false,protocol.TryDeserialize(packages[0],out frame));
            Assert.AreEqual(false,protocol.TryDeserialize(packages[1],out frame));
            Assert.AreEqual(true ,protocol.TryDeserialize(packages[2],out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame,frame);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSetsOutOfOrder_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            Frame expectedFrame = new Frame(frameIndex,frameWaitMS);
            for (int i = 0; i < 300; i++)
            {
                expectedFrame.Add(new PixelInstruction(i,1,2,3));
            }      
            
            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame,maxPackageSize);

            Assert.AreEqual(3,packages.Length);

            FrameProtocol protocol = new FrameProtocol();
            Frame frame;
            Assert.AreEqual(false,protocol.TryDeserialize(packages[0],out frame));
            Assert.AreEqual(false,protocol.TryDeserialize(packages[2],out frame));
            Assert.AreEqual(true ,protocol.TryDeserialize(packages[1],out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEquivalent(expectedFrame,frame);
        }

        [Test]
        public void GetFrameIndex_FrameAndFrameSection_GetsCorrectIndex()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            Frame expectedFrame = new Frame(frameIndex, frameWaitMS);
            for (int i = 0; i < 300; i++)
            {
                expectedFrame.Add(new PixelInstruction(i, 1, 2, 3));
            }

            // RUN
            byte[][] packages = FrameProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);
            Assert.AreEqual(frameIndex,FrameProtocol.GetFrameIndex(packages[0]));
            Assert.AreEqual(frameIndex,FrameProtocol.GetFrameIndex(packages[1]));
            Assert.AreEqual(frameIndex,FrameProtocol.GetFrameIndex(packages[2]));
        }
    }

    [TestFixture]
    public class TestFrameWithoutDeltaProtocol
    {
        [Test]
        public void SerializeFrame_Frame_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex, frameWaitMS, 3);
            frame[0] = new PixelInstructionWithoutDelta(1,2,3);
            frame[1] = new PixelInstructionWithoutDelta(4, 5, 6);
            frame[2] = new PixelInstructionWithoutDelta(7, 8, 9);

            byte[] expectedBytes = new byte[sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + 3 + 3 + 3];
            int startIndex = 0;
            BitConverter.GetBytes(frameIndex).CopyTo(expectedBytes, startIndex); // Frame index
            BitConverter.GetBytes(frameWaitMS).CopyTo(expectedBytes, startIndex += 4);
            BitConverter.GetBytes(frame.Count).CopyTo(expectedBytes, startIndex += 4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(expectedBytes, startIndex += 4); // Has FrameSections
            // instruction 1
            expectedBytes[startIndex += 1] = (byte)1;
            expectedBytes[startIndex += 1] = (byte)2;
            expectedBytes[startIndex += 1] = (byte)3;
            // instruction 2
            expectedBytes[startIndex += 1] = (byte)4;
            expectedBytes[startIndex += 1] = (byte)5;
            expectedBytes[startIndex += 1] = (byte)6;
            // instruction 3
            expectedBytes[startIndex += 1] = (byte)7;
            expectedBytes[startIndex += 1] = (byte)8;
            expectedBytes[startIndex += 1] = (byte)9;

            byte[][] packages = FrameWithoutDeltaProtocol.SerializeFrame(frame, maxPackageSize);

            Assert.AreEqual(1, packages.Length);
            Assert.AreEqual(expectedBytes, packages[0]);
        }

        [Test]
        public void SerializeFrame_FrameTooBigForASinglePackage_CreatesCorrectByteArray()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex, frameWaitMS,900);
            for (int i = 0; i < 900; i++)
            {
                frame[i] = new PixelInstructionWithoutDelta(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameWithoutDeltaProtocol.SerializeFrame(frame, maxPackageSize);

            // Deserialize
            FrameWithoutDeltaProtocol frameWithoutDeltaProtocol = new FrameWithoutDeltaProtocol();
            FrameWithoutDelta deserializedFrame = null;
            foreach (byte[] package in packages)
            {
                if(frameWithoutDeltaProtocol.TryDeserialize(package, out FrameWithoutDelta returnFrame))
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
            int frameWaitMS = 100;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(1, frameWaitMS, 3);
            expectedFrame[0] = new PixelInstructionWithoutDelta (1, 2, 3) ;
            expectedFrame[1] = new PixelInstructionWithoutDelta (4, 5, 6) ;
            expectedFrame[2] = new PixelInstructionWithoutDelta (7, 8, 9) ;

            byte[] bytes = new byte[sizeof(int) + sizeof(int) + sizeof(int) + sizeof(bool) + PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED * 3];
            int startIndex = 0;
            BitConverter.GetBytes(1).CopyTo(bytes, startIndex); // Frame index
            BitConverter.GetBytes(frameWaitMS).CopyTo(bytes, startIndex += 4); // Timestamp (relative)
            BitConverter.GetBytes(expectedFrame.Count).CopyTo(bytes, startIndex += 4); // Number of PixelInstructions
            BitConverter.GetBytes(false).CopyTo(bytes, startIndex += 4); // Has FrameSections
            // instruction 1
            bytes[startIndex += 1] = (byte)1;
            bytes[startIndex += 1] = (byte)2;
            bytes[startIndex += 1] = (byte)3;
            // instruction 2
            bytes[startIndex += 1] = (byte)4;
            bytes[startIndex += 1] = (byte)5;
            bytes[startIndex += 1] = (byte)6;
            // instruction 3
            bytes[startIndex += 1] = (byte)7;
            bytes[startIndex += 1] = (byte)8;
            bytes[startIndex += 1] = (byte)9;

            FrameWithoutDeltaProtocol protocol = new FrameWithoutDeltaProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(true, protocol.TryDeserialize(bytes, out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSets_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, frameWaitMS,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstructionWithoutDelta(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameWithoutDeltaProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);

            FrameWithoutDeltaProtocol protocol = new FrameWithoutDeltaProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(false, protocol.TryDeserialize(packages[0], out frame));
            Assert.AreEqual(false, protocol.TryDeserialize(packages[1], out frame));
            Assert.AreEqual(true, protocol.TryDeserialize(packages[2], out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEqual(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void TryDeserialize_frameWithThreeFrameSetsOutOfOrder_DeserializesCorrectly()
        {
            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, frameWaitMS,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstructionWithoutDelta(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameWithoutDeltaProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);

            FrameWithoutDeltaProtocol protocol = new FrameWithoutDeltaProtocol();
            FrameWithoutDelta frame;
            Assert.AreEqual(false, protocol.TryDeserialize(packages[0], out frame));
            Assert.AreEqual(false, protocol.TryDeserialize(packages[2], out frame));
            Assert.AreEqual(true, protocol.TryDeserialize(packages[1], out frame));
            Assert.IsNotNull(frame);
            Assert.AreEqual(expectedFrame.Index, frame.Index);
            Assert.AreEqual(expectedFrame.TimeStampRelative, frame.TimeStampRelative);
            Assert.AreEqual(expectedFrame.Count, frame.Count);
            CollectionAssert.AreEquivalent(expectedFrame.Items, frame.Items);
        }

        [Test]
        public void GetFrameIndex_FrameAndFrameSection_GetsCorrectIndex()
        {

            int maxPackageSize = 1016;
            int frameWaitMS = 100;
            int frameIndex = 9;
            FrameWithoutDelta expectedFrame = new FrameWithoutDelta(frameIndex, frameWaitMS,900);
            for (int i = 0; i < 900; i++)
            {
                expectedFrame[i] = new PixelInstructionWithoutDelta(1, 2, 3);
            }

            // RUN
            byte[][] packages = FrameWithoutDeltaProtocol.SerializeFrame(expectedFrame, maxPackageSize);

            Assert.AreEqual(3, packages.Length);
            Assert.AreEqual(frameIndex, FrameWithoutDeltaProtocol.GetFrameIndex(packages[0]));
            Assert.AreEqual(frameIndex, FrameWithoutDeltaProtocol.GetFrameIndex(packages[1]));
            Assert.AreEqual(frameIndex, FrameWithoutDeltaProtocol.GetFrameIndex(packages[2]));
        }
    }
}