using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Moq;
using NUnit.Framework;
using StellaLib.Network;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Network;

namespace StellaServerLib.Test.Network
{
    public class TestServer
    {
        [Test]
        public void ClientSendsBroadcast_NoticedByServer()
        {
            var clientMappings = new List<ClientMapping>()
            {
                new ClientMapping(0, "mac01"),
            };

            Server server = new Server();

            var socketConnectionMock = new Mock<ISocketConnection>();

            var connectionCreatorMock = new Mock<SocketConnectionCreator>();
            connectionCreatorMock
                .Setup(x => x.CreateForBroadcast(It.IsAny<int>()))
                .Returns<IPEndPoint>((localEndPoint) => socketConnectionMock.Object);
            connectionCreatorMock
                .Setup(x => x.Create(It.IsAny<IPEndPoint>()))
                .Returns<IPEndPoint>((localEndPoint) => Mock.Of<ISocketConnection>());



            server.Start(11, 22, 33, connectionCreatorMock.Object, clientMappings);

            ;
        }

    }
}
