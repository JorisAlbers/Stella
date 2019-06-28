using System;
using System.Threading;
using Moq;
using NUnit.Framework;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServerLib.Test
{
    [TestFixture]
    public class TestClientController
    {
        [Test]
        public void StartAnimation_AnimationWithOneClient_SendsAnimationStartToServer()
        {
            //SETUP
            int expectedID = 0;
            MessageType expectedMessageType = MessageType.Animation_RenderFrame;
            var mock = new Mock<IServer>();

            FrameSetMetadata frameSetMetadata = new FrameSetMetadata(DateTime.Now);
            Assert.Fail("TODO: Fix this test.");

            /*var animatorMock = new Mock<IAnimator>();
            animatorMock.Setup(x => x.GetNextFramePerPi())
                .Returns(new FrameWithoutDelta[] {new FrameWithoutDelta(0, 0),  {new PixelInstruction(1, 2, 3, 4)}});*/
            
            /*byte[] expectedBytes = FrameSetMetadataProtocol.Serialize(frameSetMetadata);
            mock.SetupGet(x=> x.ConnectedClients).Returns(new int[]{expectedID});

            int id = -1;
            MessageType messageType = MessageType.Unknown;
            byte[] bytes = null;
            mock.Setup(x=> x.SendMessageToClient(It.IsAny<int>(),
                                                 It.IsAny<MessageType>(),
                                                 It.IsAny<byte[]>()))
                                                 .Callback<int,MessageType,byte[]>((i,t,b) =>
            {
                id = i;
                messageType = t;
                bytes = b;
            });

            //EXECUTE
            ClientController controller = new ClientController(mock.Object);
            controller.Run();
            controller.StartAnimation(animatorMock.Object, DateTime.Now);

            Thread.Sleep(1000); // async hack

            //ASSERT
            Assert.AreEqual(expectedID,id);
            Assert.AreEqual(expectedMessageType,messageType);
            Assert.AreEqual(expectedBytes,bytes);*/
        }
    }
}