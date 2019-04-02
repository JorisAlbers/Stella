using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Moq;
using NUnit.Framework;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaLib.Test.Network
{
    [TestFixture]
    public class TestSocketConnectionController
    {
        [Test]
        public void Send_Message_CallsBeginSend()
        {
            MessageType messageType = MessageType.Init;
            byte[] message = Encoding.ASCII.GetBytes("test message");
            var mock = new Mock<ISocketConnection>();
            mock.Setup(x => x.Connected).Returns(true);

            byte[] expectedMessage = PacketProtocol.WrapMessage(messageType, message);

            byte[] receivedMessage = null;
            mock.Setup(x => x.BeginSend(
                It.IsAny<byte[]>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SocketFlags>(),
                It.IsAny<AsyncCallback>(),
                It.IsAny<object>()))
                .Callback<byte[],int,int,SocketFlags,AsyncCallback,object>((data, offset ,size, socketFlags, asyncCallback, state) =>
                {
                    receivedMessage = data;
                });

            SocketConnectionController controller = new SocketConnectionController(mock.Object);
            controller.Start();
            controller.Send(messageType,message);
            Assert.AreEqual(expectedMessage,receivedMessage);
        }

    }
}
