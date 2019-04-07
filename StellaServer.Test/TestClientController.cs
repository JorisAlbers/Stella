using NUnit.Framework;
using StellaServer.Network;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;
using Moq;
using System;
using StellaLib.Network;
using StellaLib.Network.Protocol;

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
            byte[] expectedBytes = FrameSetMetadataProtocol.Serialize(frameSet.Metadata);
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
            Assert.AreEqual(expectedMessageType,messageType);
            Assert.AreEqual(expectedBytes,bytes);
        }

        [Test]
        public void AnimationRequestReceived_TwoFramesRequested_SendsTheFrames()
        {
            //SETUP
            DateTime timeStamp = DateTime.Now;
            string expectedID = "ID1";
            MessageType expectedMessageType = MessageType.Animation_Request;
            FrameSet frameSet = new FrameSet(timeStamp);

            frameSet.Frames.Add(new Frame(0,100){new PixelInstruction(10,1,2,3)});
            frameSet.Frames.Add(new Frame(1,200){new PixelInstruction(20,4,5,6)});
            frameSet.Frames.Add(new Frame(2,200){new PixelInstruction(30,7,8,9)});

            byte[] expectedBytes1 = FrameProtocol.SerializeFrame(frameSet.Frames[0],PacketProtocol.MAX_MESSAGE_SIZE)[0];
            byte[] expectedBytes2 = FrameProtocol.SerializeFrame(frameSet.Frames[1],PacketProtocol.MAX_MESSAGE_SIZE)[0];
            var mock = new Mock<IServer>();
            mock.SetupGet(x=> x.ConnectedClients).Returns(new string[]{expectedID});

            string id1 = null,  id2 = null;
            MessageType messageType1 = MessageType.Unknown, messageType2 = MessageType.Unknown;
            byte[] bytes1 = null, bytes2 = null;
            int callCount = 0;

            mock.Setup(x=> x.SendMessageToClient(It.IsAny<string>(),
                                                 It.IsAny<MessageType>(),
                                                 It.IsAny<byte[]>()))
                                                 .Callback<string,MessageType,byte[]>((i,t,b) =>
            {
                if(t == MessageType.Animation_Request && callCount++ == 0)
                {
                    id1 = i;
                    messageType1 = t;
                    bytes1 = b;
                }
                else
                {
                    id2 = i;
                    messageType2 = t;
                    bytes2 = b;
                }
                
            });

            //EXECUTE
            ClientController controller = new ClientController(mock.Object);
            controller.StartAnimation(frameSet);

            mock.Raise(m => m.AnimationRequestReceived += null, new AnimationRequestEventArgs(expectedID,0,2));

            //ASSERT
            // Frame1
            Assert.AreEqual(expectedID,id1);
            Assert.AreEqual(expectedMessageType,messageType1);
            Assert.AreEqual(expectedBytes1,bytes1);
            // Frame 2            
            Assert.AreEqual(expectedID,id2);
            Assert.AreEqual(expectedMessageType,messageType2);
            Assert.AreEqual(expectedBytes2,bytes2);
        }
    }
}