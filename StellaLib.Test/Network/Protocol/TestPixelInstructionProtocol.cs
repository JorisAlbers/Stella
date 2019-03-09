using System;
using System.Drawing;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol;

namespace StellaLib.Test.Network.Protocol
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
           
            Assert.AreEqual(expectedBytes, PixelInstructionProtocol.Serialize(pi));
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
}