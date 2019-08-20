using System;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
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

            byte[] buffer = new byte[PixelInstructionProtocol.BYTES_NEEDED];
            PixelInstructionProtocol.Serialize(pi, buffer, 0);
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

            PixelInstructionWithoutDelta pi = PixelInstructionProtocol.Deserialize(bytes, 0);
            Assert.AreEqual(expectedPixelInstruction, pi);
        }

    }
}