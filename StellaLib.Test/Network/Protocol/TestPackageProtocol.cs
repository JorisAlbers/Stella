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

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);
            packetProtocol.DataReceived(message, message.Length, out Message<MessageType> messageReceived);

            Assert.AreEqual(expectedMessageType, messageReceived.MessageType);
            Assert.AreEqual(expectedBytes, messageReceived.Data);
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


            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);

            packetProtocol.DataReceived(messagePackage1, messagePackage1.Length, out Message<MessageType> messageReceived1);
            Assert.IsNull(messageReceived1);
            packetProtocol.DataReceived(messagePackage2, messagePackage2.Length, out Message<MessageType> messageReceived2);
            Assert.IsNull(messageReceived2);

            packetProtocol.DataReceived(messagePackage3, messagePackage3.Length, out Message<MessageType> messageReceived3);
            Assert.IsNull(messageReceived3);

            packetProtocol.DataReceived(messagePackage4, messagePackage4.Length, out Message<MessageType> messageReceived4);
            Assert.IsNotNull(messageReceived4);

            Assert.AreEqual(expectedMessageType, messageReceived4.MessageType);
            Assert.AreEqual(expectedBytes, messageReceived4.Data);
        }

        [Test]
        public void DataReceived_LengthShorterThanBuffer_OnlyReadsTheLength()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] { 1, 2, 3, 4 };

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);
            // Add bytes the package protocol should not read.
            byte[] tooLongMessage = message.Concat(new byte[] {9, 8, 7}).ToArray();

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(100);


            packetProtocol.DataReceived(tooLongMessage, message.Length, out Message<MessageType> messageReceived);
            Assert.AreEqual(expectedMessageType, messageReceived.MessageType);
            Assert.AreEqual(expectedBytes, messageReceived.Data);
        }

        [Test]
        public void DataReceived_PackageTooLong_ThrowsProtocolViolationException()
        {
            MessageType expectedMessageType = MessageType.Init;
            byte[] expectedBytes = new byte[] { 1, 2, 3, 4 };

            byte[] message = PacketProtocol<MessageType>.WrapMessage(expectedMessageType, expectedBytes);
            // Add bytes the package protocol should not read.

            PacketProtocol<MessageType> packetProtocol = new PacketProtocol<MessageType>(10);
            Assert.Throws<ProtocolViolationException>(() => packetProtocol.DataReceived(message, message.Length, out Message<MessageType> _));

        }

    }
}
