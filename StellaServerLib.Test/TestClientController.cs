using System;
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

            var animatorMock = new Mock<IAnimator>();
            animatorMock.Setup(x => x.GetFrameSetMetadata()).Returns(frameSetMetadata);

            byte[] expectedBytes = FrameSetMetadataProtocol.Serialize(frameSetMetadata);
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
            controller.StartAnimation(animatorMock.Object);

            //ASSERT
            Assert.AreEqual(expectedID,id);
            Assert.AreEqual(expectedMessageType,messageType);
            Assert.AreEqual(expectedBytes,bytes);
        }

        [Test]
        public void AnimationRequestReceived_TwoFramesRequested_SendsTheFrames()
        {
            //SETUP
            int expectedID = 0;
            MessageType expectedMessageType = MessageType.Animation_PrepareFrame;
            
            Frame frame1 = new Frame(0,100){new PixelInstruction(10,1,2,3)};
            Frame frame2 = new Frame(1,200){new PixelInstruction(20,4,5,6)};
            Frame frame3 = new Frame(2,200){new PixelInstruction(30,7,8,9)};

            Frame[] framesPerPi1 = new Frame[]{frame1,null,null};
            Frame[] framesPerPi2 = new Frame[]{frame2,null,null};
            Frame[] framesPerPi3 = new Frame[]{frame3,null,null};

            
            var animatorMock = new Mock<IAnimator>();
            animatorMock.Setup(x => x.GetFrameSetMetadata()).Returns(new FrameSetMetadata(DateTime.Now));
            animatorMock.SetupSequence(x => x.GetNextFramePerPi()).Returns(framesPerPi1).Returns(framesPerPi2).Returns(framesPerPi3);

            byte[] expectedBytes1 = FrameProtocol.SerializeFrame(frame1,PacketProtocol<MessageType>.MAX_MESSAGE_SIZE)[0];
            byte[] expectedBytes2 = FrameProtocol.SerializeFrame(frame2,PacketProtocol<MessageType>.MAX_MESSAGE_SIZE)[0];
            var mock = new Mock<IServer>();
            mock.SetupGet(x=> x.ConnectedClients).Returns(new int[]{expectedID});

            byte[] bytes1 = null, bytes2 = null;
            int callCount = 0;

            mock.Setup(x=> x.SendMessageToClient(0,
                                                 MessageType.Animation_PrepareFrame,
                                                 It.IsAny<byte[]>()))
                                                 .Callback<int,MessageType,byte[]>((i,t,b) =>
            {
                if(callCount++ == 0)
                {
                    bytes1 = b;
                }
                else
                {
                    bytes2 = b;
                }
                
            });

            //EXECUTE
            ClientController controller = new ClientController(mock.Object);
            controller.StartAnimation(animatorMock.Object);

            mock.Raise(m => m.AnimationRequestReceived += null, new AnimationRequestEventArgs(expectedID,0,2));

            //ASSERT
            Assert.AreEqual(2, callCount);
            // Frame1
            Assert.AreEqual(expectedBytes1,bytes1);
            // Frame 2            
            Assert.AreEqual(expectedBytes2,bytes2);
        }
    }
}