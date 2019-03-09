using System;
using System.Drawing;
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
            int frameWaitMS = 100;
            Frame frame = new Frame(frameWaitMS)
            { 
                new PixelInstruction{ Index = 1,   Color = Color.FromArgb(1,2,3)},
                new PixelInstruction{ Index = 2,   Color = Color.FromArgb(4,5,6)},
                new PixelInstruction{ Index = 10,  Color = Color.FromArgb(7,8,9)}
            };
            byte[] expectedBytes = new byte[sizeof(int)+ sizeof(int) + sizeof(int)+3 + sizeof(int)+3 + sizeof(int)+3];
            BitConverter.GetBytes(3).CopyTo(expectedBytes,0); 
            BitConverter.GetBytes(frameWaitMS).CopyTo(expectedBytes,4); 
            // instruction 1
            BitConverter.GetBytes(1).CopyTo(expectedBytes,8);
            expectedBytes[12] = (byte)1;
            expectedBytes[13] = (byte)2;
            expectedBytes[14] = (byte)3;
            // instruction 2
            BitConverter.GetBytes(2).CopyTo(expectedBytes,15);
            expectedBytes[19] = (byte)4;
            expectedBytes[20] = (byte)5;
            expectedBytes[21] = (byte)6;
            // instruction 3
            BitConverter.GetBytes(10).CopyTo(expectedBytes,22);
            expectedBytes[26] = (byte)7;
            expectedBytes[27] = (byte)8;
            expectedBytes[28] = (byte)9;

            byte[] frameBytes = FrameProtocol.SerializeFrame(frame);
            Assert.AreEqual(expectedBytes, frameBytes);
        }

        [Test]
        public void DataReceived_FrameAsAsBytes_FiresFrameReceivedAction()
        {
            Frame frame = new Frame(100)
            { 
                new PixelInstruction{ Index = 1,   Color = Color.FromArgb(1,2,3)},
                new PixelInstruction{ Index = 2,   Color = Color.FromArgb(4,5,6)},
                new PixelInstruction{ Index = 10,  Color = Color.FromArgb(7,8,9)}
            };
            byte[] bytes = FrameProtocol.SerializeFrame(frame);

            bool receivedFrameTrigger = false;

            FrameProtocol protocol = new FrameProtocol();
            protocol.ReceivedFrame = (f)=> 
            {
                CollectionAssert.AreEqual(frame,f);
                receivedFrameTrigger = true;
            };
            protocol.DataReceived(bytes);
            Assert.IsTrue(receivedFrameTrigger);
        }

        [Test]
        public void DataReceived_FrameAsMultipleByteArrays_FiresFrameReceivedAction()
        {
            Frame frame = new Frame(100)
            { 
                new PixelInstruction{ Index = 1,   Color = Color.FromArgb(1,2,3)},
                new PixelInstruction{ Index = 2,   Color = Color.FromArgb(4,5,6)},
                new PixelInstruction{ Index = 10,  Color = Color.FromArgb(7,8,9)}
            };
            byte[] bytes = FrameProtocol.SerializeFrame(frame);

            // Split the bytes up to fake a large frame that has to be send over multiple packages
            byte[] array1 = new byte[6];
            array1[0] = bytes[0];
            array1[1] = bytes[1];
            array1[2] = bytes[2];
            array1[3] = bytes[3];
            array1[4] = bytes[4];
            array1[5] = bytes[5];

            byte[] array2 = new byte[6];
            array2[0] = bytes[6];
            array2[1] = bytes[7];
            array2[2] = bytes[8];
            array2[3] = bytes[9];
            array2[4] = bytes[10];
            array2[5] = bytes[11];

            byte[] array3 = bytes.Skip(12).ToArray();
            
            bool receivedFrameTrigger = false;

            FrameProtocol protocol = new FrameProtocol();
            protocol.ReceivedFrame = (f)=> 
            {
                CollectionAssert.AreEqual(frame,f);
                receivedFrameTrigger = true;
            };
            protocol.DataReceived(array1);
            protocol.DataReceived(array2);
            protocol.DataReceived(array3);
            Assert.IsTrue(receivedFrameTrigger);
        }
    }
}