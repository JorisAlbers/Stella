using System;
using System.Drawing;
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
            PixelInstruction pi = new PixelInstruction();
            pi.Index = 10;
            pi.Color = Color.FromArgb(1,2,3);           
            
            byte[] expectedBytes = new byte[sizeof(int)+ 1 + 1 + 1]; // Index, red, green , blue 
            BitConverter.GetBytes(pi.Index).CopyTo(expectedBytes,0);
            expectedBytes[4] = (byte)pi.Color.R;
            expectedBytes[5] = (byte)pi.Color.G;
            expectedBytes[6] = (byte)pi.Color.B;
           
            byte[] buffer = new byte[PixelInstructionProtocol.BYTES_NEEDED];
            PixelInstructionProtocol.Serialize(pi,buffer,0);
            Assert.AreEqual(expectedBytes, buffer );
        }
        
        [Test]
        public void Deserialize_BytesArray_CorrectlyDeserializesByteArray()
        {
            PixelInstruction expectedPixelInstruction = new PixelInstruction();
            expectedPixelInstruction.Index = 10;
            expectedPixelInstruction.Color = Color.FromArgb(1,2,3); 

            byte[] bytes = new byte[sizeof(int)+ 1 + 1 + 1]; // Index, red, green , blue 
            BitConverter.GetBytes(expectedPixelInstruction.Index).CopyTo(bytes,0);
            bytes[4] = (byte)expectedPixelInstruction.Color.R;
            bytes[5] = (byte)expectedPixelInstruction.Color.G;
            bytes[6] = (byte)expectedPixelInstruction.Color.B;

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
            PixelInstructionWithoutDelta pi = new PixelInstructionWithoutDelta();
            pi.Color = Color.FromArgb(1, 2, 3);

            byte[] expectedBytes = new byte[1 + 1 + 1]; // Red, green , blue 
            expectedBytes[0] = (byte)pi.Color.R;
            expectedBytes[1] = (byte)pi.Color.G;
            expectedBytes[2] = (byte)pi.Color.B;

            byte[] buffer = new byte[PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED];
            PixelInstructionWithoutDeltaProtocol.Serialize(pi, buffer, 0);
            Assert.AreEqual(expectedBytes, buffer);
        }

        [Test]
        public void Deserialize_BytesArray_CorrectlyDeserializesByteArray()
        {
            PixelInstructionWithoutDelta expectedPixelInstruction = new PixelInstructionWithoutDelta();
            expectedPixelInstruction.Color = Color.FromArgb(1, 2, 3);

            byte[] bytes = new byte[1 + 1 + 1]; // Red, green , blue 
            bytes[0] = (byte)expectedPixelInstruction.Color.R;
            bytes[1] = (byte)expectedPixelInstruction.Color.G;
            bytes[2] = (byte)expectedPixelInstruction.Color.B;

            PixelInstructionWithoutDelta pi = PixelInstructionWithoutDeltaProtocol.Deserialize(bytes, 0);
            Assert.AreEqual(expectedPixelInstruction, pi);
        }

    }
}