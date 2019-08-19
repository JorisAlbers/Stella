using System;
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

            byte[] expectedMessage = PacketProtocol<MessageType>.WrapMessage(messageType, message);

            byte[] receivedMessage = null;
            mock.Setup(x => x.BeginSend(
                It.IsAny<byte[]>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SocketFlags>(),
                It.IsAny<AsyncCallback>(),
                It.IsAny<object>()))
                .Callback<byte[], int, int, SocketFlags, AsyncCallback, object>((data, offset, size, socketFlags, asyncCallback, state) =>
                     {
                         receivedMessage = data;
                     });

            SocketConnectionController<MessageType> controller = new SocketConnectionController<MessageType>(mock.Object, 1024);
            controller.Start();
            controller.Send(messageType, message);
            Assert.AreEqual(expectedMessage, receivedMessage);
        }

        [Test]
        public void SendCallback_SocketException_InvokesOnDisconnect()
        {
            MessageType messageType = MessageType.Init;
            byte[] message = Encoding.ASCII.GetBytes("test message");
            byte[] expectedMessage = PacketProtocol<MessageType>.WrapMessage(messageType, message);

            var mock = new Mock<ISocketConnection>();
            mock.Setup(x => x.Connected).Returns(true);

            // Chain the BeginSend to the EndSend SendCallBack function
            mock.Setup(x => x.BeginSend(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<SocketFlags>(),
                    It.IsAny<AsyncCallback>(),
                    It.IsAny<object>()))
                .Callback<byte[], int, int, SocketFlags, AsyncCallback, object>((data, offset, size, socketFlags, asyncCallback, state) =>
                {
                    asyncCallback.Invoke(null);
                });

            mock.Setup(x => x.EndSend(It.IsAny<IAsyncResult>())).Throws<SocketException>();
            // Listen to the Disconnect event
            bool disconnectInvoked = false;
            SocketConnectionController<MessageType> controller = new SocketConnectionController<MessageType>(mock.Object, 1024);
            controller.Disconnect += (sender, args) => disconnectInvoked = true;

            // Run
            controller.Start();
            controller.Send(messageType, message);
            Assert.IsTrue(disconnectInvoked);
        }

        [Test]
        public void ReceiveCallback_Message_InvokesMessageArrived()
        {
            // The receive callback is private. We have to chain the receive via the start function.
            MessageType messageType = MessageType.Init;
            byte[] message = Encoding.ASCII.GetBytes("test message");
            var mock = new Mock<ISocketConnection>();
            mock.Setup(x => x.Connected).Returns(true);

            byte[] dataToSend = PacketProtocol<MessageType>.WrapMessage(messageType, message);


            var asyncStateMock = new Mock<IAsyncResult>();
            asyncStateMock.Setup(x => x.AsyncState).Returns(dataToSend);

            bool beginReceiveHasBeenInvoked = false;
            mock.Setup(x => x.BeginReceive(
                    It.IsAny<byte[]>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<SocketFlags>(),
                    It.IsAny<AsyncCallback>(),
                    It.IsAny<object>()))
                .Callback<byte[], int, int, SocketFlags, AsyncCallback, object>((data, offset, size, socketFlags, asyncCallback, state) =>
               {
                   if (!beginReceiveHasBeenInvoked)
                   {
                       beginReceiveHasBeenInvoked = true; // Prevent endless loop
                        asyncCallback.Invoke(asyncStateMock.Object); // calls ReceiveCallBack
                    }
               });

            bool endSendCalled = false;
            mock.Setup(x => x.EndReceive(It.IsAny<IAsyncResult>())).Returns(dataToSend.Length).Callback(() => endSendCalled = true);

            SocketConnectionController<MessageType> controller = new SocketConnectionController<MessageType>(mock.Object, 1024);
            MessageReceivedEventArgs<MessageType> messageReceivedEventArgs = null;
            controller.MessageReceived += (sender, args) => { messageReceivedEventArgs = args; };
            controller.Start();
            Assert.IsTrue(endSendCalled);
            Assert.AreEqual(messageType, messageReceivedEventArgs.MessageType);
            Assert.AreEqual(message, messageReceivedEventArgs.Message);
        }
    }
}
