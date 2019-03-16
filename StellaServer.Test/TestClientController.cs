using NUnit.Framework;
using StellaServer.Network;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;
using Moq;
using System;
using StellaLib.Network;

namespace StellaServer.Test
{
    [TestFixture]
    public class TestClientController
    {
        [Test]
        public void StartAnimation_AnimationWithOneClient_SendsAnimationStartToServer()
        {
            //SETUP
            DateTime timeStamp = DateTime.Now;
            string expectedID = "ID1";
            MessageType expectedMessageType = MessageType.Animation_Start;
            var mock = new Mock<IServer>();
            FrameSet frameSet = new FrameSet(timeStamp);
            byte[] expectedBytes = FrameSetProtocol.Serialize(frameSet);
            mock.SetupGet(x=> x.ConnectedClients).Returns(new string[]{expectedID});

            string id = null;
            MessageType messageType = MessageType.Unknown;
            byte[] bytes = null;
            mock.Setup(x=> x.SendMessageToClient(It.IsAny<string>(),
                                                 It.IsAny<MessageType>(),
                                                 It.IsAny<byte[]>()))
                                                 .Callback<string,MessageType,byte[]>((i,t,b) =>
            {
                id = i;
                messageType = t;
                bytes = b;
            });

            //EXECUTE
            ClientController controller = new ClientController(mock.Object);
            controller.StartAnimation(frameSet);

            //ASSERT
            Assert.AreEqual(expectedID,id);
            Assert.AreEqual(expectedMessageType,expectedMessageType);
            Assert.AreEqual(expectedBytes,bytes);
        }
    }
}