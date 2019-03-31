using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using StellaClient.Light;
using StellaClient.Network;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

namespace StellaClient.Test
{
    [TestFixture]
    public class TestRpiController
    {
        [Test]
        public void _ServerAnimationRequestReceived_SendsToLedController()
        {
            var stellaServerMock = new Mock<IStellaServer>();
            var ledControllerMock = new Mock<ILedController>();

            FrameSetMetadata expectedFrameSetMetadata = new FrameSetMetadata(DateTime.Now);

            FrameSetMetadata receivedMetaData = null;
            ledControllerMock.Setup(x => x.PrepareNextFrameSet(It.IsAny<FrameSetMetadata>()))
                .Callback<FrameSetMetadata>((metadata) => { receivedMetaData = metadata; });

            RpiController controller = new RpiController(stellaServerMock.Object, ledControllerMock.Object);
            stellaServerMock.Raise(x=> x.AnimationStartReceived += null,null,expectedFrameSetMetadata);
            Assert.AreEqual(expectedFrameSetMetadata,receivedMetaData);
        }

        [Test]
        public void _LedControllerFramesNeeded_SendsToStellaServer()
        {
            var stellaServerMock = new Mock<IStellaServer>();
            var ledControllerMock = new Mock<ILedController>();

            FramesNeededEventArgs expectedFramesNeededEventArgs = new FramesNeededEventArgs(10,30);

            int? receivedLastFrameIndex = null;
            int receivedCount = -1;
            stellaServerMock.Setup(x => x.SendFrameRequest(It.IsAny<int?>(), It.IsAny<int>())).Callback<int?, int>(
                (lastFrameIndex, count) =>
                {
                    receivedLastFrameIndex = lastFrameIndex;
                    receivedCount = count;
                });


            RpiController controller = new RpiController(stellaServerMock.Object, ledControllerMock.Object);
            ledControllerMock.Raise(x => x.FramesNeeded += null, null, expectedFramesNeededEventArgs);
            Assert.AreEqual(expectedFramesNeededEventArgs.LastFrameIndex, receivedLastFrameIndex);
            Assert.AreEqual(expectedFramesNeededEventArgs.Count, receivedCount);
        }
    }
}
