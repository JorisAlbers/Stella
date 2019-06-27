using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using StellaClient.Network;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using Moq;
using System.Text;

namespace StellaClient.Test.Network
{
    [TestFixture]
    public class TestStellaServer
    {
        [Test]
        public void Send_message_SendsMessage()
        {
             // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);  

            // Create a server
            Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
            server.Bind(localEndPoint);  
            server.Listen(2);

            // Create a SocketConnectionController
            int clientId = 0;
            StellaServer stellaServer = new StellaServer(localEndPoint, 20056, clientId);
            stellaServer.Start();
            
            Socket server_receiver = server.Accept();
            Thread.Sleep(1000);

            byte[] message = Encoding.ASCII.GetBytes("ThisIsAMessage");
            byte[] expectedInit = PacketProtocol<MessageType>.WrapMessage(MessageType.Init, BitConverter.GetBytes(clientId));
            byte[] expectedMessage = PacketProtocol<MessageType>.WrapMessage(MessageType.Standard, message);

            // Expected init message
            byte[] receiveBufferInitMessage = new byte[expectedInit.Length];
            server_receiver.Receive(receiveBufferInitMessage);
            Assert.AreEqual(expectedInit, receiveBufferInitMessage);

            // Expected Standard message
            stellaServer.Send(MessageType.Standard, message);
            byte[] receiveBufferMessage = new byte[expectedMessage.Length];
            server_receiver.Receive(receiveBufferMessage);
            Assert.AreEqual(expectedMessage, receiveBufferMessage);

        }
    }
}