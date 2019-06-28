using System.Linq;
using System.Net;
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

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);
            packetProtocol.MessageArrived += (type, bytes) =>
            {
                receivedMessageType = type;
                receivedBytes = bytes;
            };

            packetProtocol.DataReceived(message, message.Length);

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

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);
            packetProtocol.MessageArrived += (type, bytes) =>
            {
                receivedMessageType = type;
                receivedBytes = bytes;
            };

            packetProtocol.DataReceived(messagePackage1, messagePackage1.Length);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);
            packetProtocol.DataReceived(messagePackage2, messagePackage2.Length);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);
            packetProtocol.DataReceived(messagePackage3, messagePackage3.Length);
            Assert.AreEqual(MessageType.Unknown, receivedMessageType);

            packetProtocol.DataReceived(messagePackage4, messagePackage4.Length);

            Assert.AreEqual(expectedMessageType, receivedMessageType);
            Assert.AreEqual(expectedBytes, receivedBytes);
        }

        [Test]
        public void DataReceived_LengthShorterThanBuffer_OnlyReadsTheLength()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] { 1, 2, 3, 4 };

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);
            // Add bytes the package protocol should not read.
            byte[] tooLongMessage = message.Concat(new byte[] {9, 8, 7}).ToArray();

            MessageType receivedMessageType = MessageType.Unknown;
            byte[] receivedBytes = null;

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);
            packetProtocol.MessageArrived += (type, bytes) =>
            {
                receivedMessageType = type;
                receivedBytes = bytes;
            };

            packetProtocol.DataReceived(tooLongMessage, message.Length);
            Assert.AreEqual(expectedMessageType, receivedMessageType);
            Assert.AreEqual(expectedBytes, receivedBytes);
        }

        [Test]
        public void DataReceived_PackageTooLong_ThrowsProtocolViolationException()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] { 1, 2, 3, 4 };

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);
            // Add bytes the package protocol should not read.

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(10);
            Assert.Throws<ProtocolViolationException>(() => packetProtocol.DataReceived(message, message.Length));

        }

    }
}
