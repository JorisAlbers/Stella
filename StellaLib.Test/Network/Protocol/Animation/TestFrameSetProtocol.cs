using System;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaLib.Test.Network.Protocol.Animation
{
    [TestFixture]
    public class TestFrameSetProtocol
    {
        [Test]
        public void Serialize_FrameSet_CorrectlySerializes()
        {
            DateTime expectedTimeStamp = DateTime.Now;
            FrameSet frameSet = new FrameSet(expectedTimeStamp);
            
            byte[] expectedBytes = new byte[sizeof(long) + sizeof(int)];
            BitConverter.GetBytes(frameSet.TimeStamp.Ticks).CopyTo(expectedBytes,0);
            BitConverter.GetBytes(frameSet.Count).CopyTo(expectedBytes,sizeof(long));

            byte[] bytes = FrameSetProtocol.Serialize(frameSet);

            Assert.AreEqual(expectedBytes,bytes);
        }

        [Test]
        public void Deserialze_bytes_CorrectlyDeserializes()
        {
            DateTime expectedTimeStamp = DateTime.Now;
            int expectedCount = 1;

            byte[] bytes = new byte[sizeof(long) + sizeof(int)];
            BitConverter.GetBytes(expectedTimeStamp.Ticks).CopyTo(bytes,0);
            BitConverter.GetBytes(expectedCount).CopyTo(bytes,sizeof(long));

            FrameSet frameSet = FrameSetProtocol.Deserialize(bytes);
            Assert.AreEqual(expectedTimeStamp,frameSet.TimeStamp);
            // Assert.AreEqual(expectedCount,frameSet.Count); TODO implement count in deserialisation

        }
        
    }
}