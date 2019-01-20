using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;
using StellaClient.Network;
using StellaLib.Network;
using StellaLib.Network.Protocol;

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

            // Create a SocketConnection
            StellaServer stellaServer = new StellaServer(localEndPoint, "ThisIsAnID");
            stellaServer.Start();
            
            Socket server_receiver = server.Accept();
            Thread.Sleep(1000);

            byte[] expected = PacketProtocol.WrapMessage(MessageType.Standard, "ThisIsAMessage");

            stellaServer.Send(MessageType.Standard, "ThisIsAMessage");
            byte[] receiveBuffer = new byte[expected.Length];
            server_receiver.Receive(receiveBuffer);
            Assert.AreEqual(expected,receiveBuffer);
        }
    }
}