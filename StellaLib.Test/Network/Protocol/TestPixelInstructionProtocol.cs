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
            BitConverter.GetBytes(10).CopyTo(expectedBytes,0);
            expectedBytes[4] = (byte)1;
            expectedBytes[5] = (byte)2;
            expectedBytes[6] = (byte)3;
           
            Assert.AreEqual(expectedBytes, PixelInstructionProtocol.Serialize(pi));
        }
        
    }
}