using NUnit.Framework;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaLib.Test.Network.Protocol
{
    [TestFixture]
    public class TestPackageProtocol
    {
        [Test]
        public void DataReceived_FullMessage_ReturnsCorrectMessage()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] {1, 2, 3, 4};

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);

            MessageType receivedMessageType = MessageType.Unknown;
            byte[] receivedBytes = null;

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>();
            packetProtocol.MessageArrived += (type, bytes) =>
            {
                receivedMessageType = type;
                receivedBytes = bytes;
            };

            packetProtocol.DataReceived(message);

            Assert.AreEqual(expectedMessageType, receivedMessageType);
            Assert.AreEqual(expectedBytes, receivedBytes);
        }
    }
}
