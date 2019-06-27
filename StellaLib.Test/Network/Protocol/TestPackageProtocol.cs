using System.Linq;
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

        [Test]
        public void DataReceived_MessageSplitInMultiplePackages_ReturnsCorrectMessage()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] { 1, 2, 3, 4 };

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);

            Assert.AreEqual(12, message.Length, "This test assumes the message length is 12");

            byte[] messagePackage1 = {message[0],
                                      message[1],
                                      message[2]};
            byte[] messagePackage2 = {message[3],
                                      message[4],
                                      message[5]};
            byte[] messagePackage3 = {message[6],
                                      message[7],
                                      message[8]};
            byte[] messagePackage4 = {message[9],
                                      message[10],
                                      message[11],
            };

            MessageType receivedMessageType = MessageType.Unknown;
            byte[] receivedBytes = null;

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>();
            packetProtocol.MessageArrived += (type, bytes) =>
            {
                receivedMessageType = type;
                receivedBytes = bytes;
            };

            packetProtocol.DataReceived(messagePackage1);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);
            packetProtocol.DataReceived(messagePackage2);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);
            packetProtocol.DataReceived(messagePackage3);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);

            packetProtocol.DataReceived(messagePackage4);

            Assert.AreEqual(expectedMessageType, receivedMessageType);
            Assert.AreEqual(expectedBytes, receivedBytes);
        }

    }
}
