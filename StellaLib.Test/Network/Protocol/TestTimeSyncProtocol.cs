using System;
using NUnit.Framework;
using StellaLib.Network.Protocol;


namespace StellaLib.Test.Network.Protocol
{
    [TestFixture]
    public class TestTimeSyncProtocol
    {
        [Test]
        public void CreateMessage_noParameters_CreatesCorrectMessage()
        {
            DateTime now = DateTime.Now;
            byte[] expectedBytes = BitConverter.GetBytes(now.Ticks);
            Assert.AreEqual(expectedBytes,TimeSyncProtocol.CreateMessage(now));
        }

        [Test]
        public void CreateMessage_previousMessage_CreatesCorrectMessage()
        {
            DateTime then = DateTime.Now - TimeSpan.FromMinutes(2);
            DateTime now = DateTime.Now;
            byte[] expectedBytes = new byte[16];
            BitConverter.GetBytes(then.Ticks).CopyTo(expectedBytes,0);
            BitConverter.GetBytes(now.Ticks).CopyTo(expectedBytes,sizeof(long));

            byte[] previousMessage = BitConverter.GetBytes(then.Ticks);

            Assert.AreEqual(expectedBytes,TimeSyncProtocol.CreateMessage(now, previousMessage));
        }

        [Test]
        public void ParseMessage_bytes_CorrectlyParsesMessage()
        {
            long m1 = long.MaxValue, m2 = 987654321, m3 = 87654321;
            byte[] bytes = new byte[3*sizeof(long)];

            BitConverter.GetBytes(m1).CopyTo(bytes, 0);
            BitConverter.GetBytes(m2).CopyTo(bytes, sizeof(long));
            BitConverter.GetBytes(m3).CopyTo(bytes, 2*sizeof(long));
            CollectionAssert.AreEqual(new long[]{m1,m2,m3}, TimeSyncProtocol.ParseMessage(bytes));
        }
    }
}
