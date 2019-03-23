using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using StellaLib.Network;
using StellaLib.Network.Protocol;

namespace StellaLib.Test.Network.Protocol
{
    [TestFixture]
    public class TestSocketConnection
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
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); 
            socket.Connect(localEndPoint);

            SocketConnection socketConnection = new SocketConnection(socket);
            socketConnection.Start();
            
            Socket server_receiver = server.Accept();
            Thread.Sleep(1000);
            byte[] message = Encoding.ASCII.GetBytes("ThisIsAMessage");
            byte[] expectedData = PacketProtocol.WrapMessage(MessageType.Standard,message);

            socketConnection.Send(MessageType.Standard, message);
            byte[] receiveBuffer = new byte[expectedData.Length];
            server_receiver.Receive(receiveBuffer);
            Assert.AreEqual(expectedData,receiveBuffer);
        }
    }
}