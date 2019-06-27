using System;
using System.Collections.Generic;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol
{
    [TestFixture]
    public class TestAnimationRequestProtocol
    {
        [Test]
        public void CreateRequest_Request_SerializesCorrectly()
        {
            int expectedStartIndex = 6;
            int expectedCount = 9;

            byte[] expectedBytes = new byte[sizeof(int) + sizeof(int)];
            BitConverter.GetBytes(expectedStartIndex).CopyTo(expectedBytes,0);
            BitConverter.GetBytes(expectedCount).CopyTo(expectedBytes,sizeof(int));
            Assert.AreEqual(expectedBytes,AnimationRequestProtocol.CreateRequest(expectedStartIndex,expectedCount));
        }

        [Test]
        public void ParseRequest_Request_DeserializesCorrectly()
        {
            int expectedStartIndex = 6;
            int expectedCount = 9;

            byte[] bytes = new byte[sizeof(int) + sizeof(int)];
            BitConverter.GetBytes(expectedStartIndex).CopyTo(bytes,0);
            BitConverter.GetBytes(expectedCount).CopyTo(bytes,sizeof(int));

            int startIndex;
            int count;
            AnimationRequestProtocol.ParseRequest(bytes,out startIndex, out count);

            Assert.AreEqual(expectedStartIndex,startIndex);
            Assert.AreEqual(expectedCount,count);
        }

        [Test]
        public void CreateRespons_singleFrame_SerializesCorrectly()
        {
            Frame frame = new Frame(9,100){new PixelInstruction(10,1,2,3)};
            byte[][] expectedBytes = FrameProtocol.SerializeFrame(frame,1016);
            
            List<byte[]> bytes = AnimationRequestProtocol.CreateResponse(new Frame[]{frame}, 1024);

            Assert.AreEqual(expectedBytes.Length,bytes.Count);
            Assert.AreEqual(expectedBytes[0],bytes[0]);
        }
    }

    
}