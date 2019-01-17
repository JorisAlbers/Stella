using System;
using System.Drawing;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol;

namespace StellaLib.Test.Network.Protocol
{
    [TestFixture]
    public class TestFrameSetProtocol
    {
        [Test]
        public void SerializeFrame_FrameSet_CreatesCorrectByteArray()
        {
            Frame frame = new Frame
            { 
                new PixelInstruction{ Index = 1,   Color = Color.FromArgb(1,2,3)},
                new PixelInstruction{ Index = 2,   Color = Color.FromArgb(4,5,6)},
                new PixelInstruction{ Index = 10,  Color = Color.FromArgb(7,8,9)}
            };
            byte[] expectedBytes = new byte[sizeof(int)+ sizeof(int)+3 + sizeof(int)+3 + sizeof(int)+3];
            BitConverter.GetBytes(3).CopyTo(expectedBytes,0); 
            // instruction 1
            BitConverter.GetBytes(1).CopyTo(expectedBytes,4);
            expectedBytes[8] = (byte)1;
            expectedBytes[9] = (byte)2;
            expectedBytes[10] = (byte)3;
            // instruction 1
            BitConverter.GetBytes(2).CopyTo(expectedBytes,11);
            expectedBytes[15]  = (byte)4;
            expectedBytes[16] = (byte)5;
            expectedBytes[17] = (byte)6;
            // instruction 1
            BitConverter.GetBytes(10).CopyTo(expectedBytes,18);
            expectedBytes[22] = (byte)7;
            expectedBytes[23] = (byte)8;
            expectedBytes[24] = (byte)9;

            Assert.AreEqual(expectedBytes, FrameSetProtocol.SerializeFrame(frame));
        }
    }
}