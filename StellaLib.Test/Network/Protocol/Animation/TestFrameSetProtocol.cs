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
            
            byte[] expectedBytes = new byte[sizeof(long)];
            BitConverter.GetBytes(frameSet.TimeStamp.Ticks).CopyTo(expectedBytes,0);

            byte[] bytes = FrameSetMetadataProtocol.Serialize(frameSet.Metadata);

            Assert.AreEqual(expectedBytes,bytes);
        }

        [Test]
        public void Deserialze_bytes_CorrectlyDeserializes()
        {
            DateTime expectedTimeStamp = DateTime.Now;

            byte[] bytes = new byte[sizeof(long)];
            BitConverter.GetBytes(expectedTimeStamp.Ticks).CopyTo(bytes,0);

            FrameSetMetadata metadata = FrameSetMetadataProtocol.Deserialize(bytes);
            Assert.AreEqual(expectedTimeStamp,metadata.TimeStamp);

        }
        
    }
}