using System;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
    [TestFixture]
    public class TestPixelInstructionProtocol
    {
        [Test]
        public void Serialize_PixelinStruction_CorrectlyConvertedToByteArray()
        {
            PixelInstruction pi = new PixelInstruction(10,1,2,3);

            byte[] expectedBytes = new byte[sizeof(int)+ 1 + 1 + 1]; // Index, red, green , blue 
            BitConverter.GetBytes(pi.Index).CopyTo(expectedBytes,0);
            expectedBytes[4] = pi.R;
            expectedBytes[5] = pi.G;
            expectedBytes[6] = pi.B;
           
            byte[] buffer = new byte[PixelInstructionProtocol.BYTES_NEEDED];
            PixelInstructionProtocol.Serialize(pi,buffer,0);
            Assert.AreEqual(expectedBytes, buffer );
        }
        
        [Test]
        public void Deserialize_BytesArray_CorrectlyDeserializesByteArray()
        {
            PixelInstruction expectedPixelInstruction = new PixelInstruction(10,1,2,3);

            byte[] bytes = new byte[sizeof(int)+ 1 + 1 + 1]; // Index, red, green , blue 
            BitConverter.GetBytes(expectedPixelInstruction.Index).CopyTo(bytes,0);
            bytes[4] = expectedPixelInstruction.R;
            bytes[5] = expectedPixelInstruction.G;
            bytes[6] = expectedPixelInstruction.B;

            PixelInstruction pi = PixelInstructionProtocol.Deserialize(bytes,0);
            Assert.AreEqual(expectedPixelInstruction,pi);
        }
        
    }

    [TestFixture]
    public class TestPixelInstructionWithoutDeltaProtocol
    {
        [Test]
        public void Serialize_PixelInStructionWithoutDelta_CorrectlyConvertedToByteArray()
        {
            PixelInstructionWithoutDelta pi = new PixelInstructionWithoutDelta(1,2,3);

            byte[] expectedBytes = new byte[1 + 1 + 1]; // Red, green , blue 
            expectedBytes[0] = pi.R;
            expectedBytes[1] = pi.G;
            expectedBytes[2] = pi.B;

            byte[] buffer = new byte[PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED];
            PixelInstructionWithoutDeltaProtocol.Serialize(pi, buffer, 0);
            Assert.AreEqual(expectedBytes, buffer);
        }

        [Test]
        public void Deserialize_BytesArray_CorrectlyDeserializesByteArray()
        {
            PixelInstructionWithoutDelta expectedPixelInstruction = new PixelInstructionWithoutDelta(1,2,3);

            byte[] bytes = new byte[1 + 1 + 1]; // Red, green , blue 
            bytes[0] = expectedPixelInstruction.R;
            bytes[1] = expectedPixelInstruction.G;
            bytes[2] = expectedPixelInstruction.B;

            PixelInstructionWithoutDelta pi = PixelInstructionWithoutDeltaProtocol.Deserialize(bytes, 0);
            Assert.AreEqual(expectedPixelInstruction, pi);
        }

    }
}