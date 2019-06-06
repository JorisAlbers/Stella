using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaServerLib.Network;

namespace StellaServer.Test.Animation.Network
{
    [TestFixture]
    public class TestServer
    {
        [Ignore("a")]
        [Test]
        public void SendDataToClient_DefaultMessage_MessageSent()
        {
            Server server = new Server("localhost", 20055);
            server.Start();

            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20055);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            client.Connect( remoteEP);  
            Thread.Sleep(1000); // async hack
            int ID = 0;
            byte[] id_bytes = BitConverter.GetBytes(ID);
            // Then send the init values
            client.Send(PacketProtocol.WrapMessage(MessageType.Init, id_bytes));
            Thread.Sleep(10000); // async hack

            client.Receive(new byte[1024]); // retrieve and skip the INIT message
            
            byte[] message = Encoding.ASCII.GetBytes("ThisIsAMessage");
            server.SendMessageToClient(ID,MessageType.Standard,message);

            byte[] buffer = new byte[1024];
            int bytesRead = client.Receive(buffer);
            
            byte[] expectedBytes = PacketProtocol.WrapMessage(MessageType.Standard,message);
            Assert.AreEqual(expectedBytes, buffer.Take(bytesRead).ToArray());
            server.Dispose();
        }

        [Ignore("a")]
        [Test]
        public void InitMessageSent_ExistingClient_ClientReplaced()
        {
            Server server = new Server("localhost", 20055);
            server.Start();

            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);  

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20055);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            client.Connect( remoteEP);  
            Thread.Sleep(1000); // async hack

            byte[] buffer = new byte[1024];
            int bytesReceived = client.Receive(buffer); // retrieve the INIT message
            byte[] expectedInitRequest = PacketProtocol.WrapMessage(MessageType.Init,new byte[0]);
            Assert.AreEqual(expectedInitRequest,buffer.Take(bytesReceived).ToArray());

            string expectedID = "ThisIsAnIdentifier";
            byte[] expectedIDBytes = Encoding.ASCII.GetBytes(expectedID);
            byte[] message = PacketProtocol.WrapMessage(MessageType.Init, expectedIDBytes);
            // Then send the init values
            client.Send(message);
            Thread.Sleep(1000); // async hack

            Socket client2 = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client2.Connect(remoteEP);
            Thread.Sleep(1000);

            Assert.AreEqual(1,server.NewConnectionsCount);
            Assert.AreEqual(1,server.ConnectedClients.Length);
            Assert.AreEqual(expectedID,server.ConnectedClients[0]);

            client2.Send(message);
            Thread.Sleep(1000); // async hack

            Assert.AreEqual(0,server.NewConnectionsCount);
            Assert.AreEqual(1,server.ConnectedClients.Length);
            Assert.AreEqual(expectedID,server.ConnectedClients[0]);

            server.Dispose();
        }

        [Ignore("a")]
        [Test]
        public void InitMessageSent_NewClient_ClientGetsMovedToListOfClients()
        {
            Server server = new Server("localhost",20055);
            server.Start();

            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);  

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20055);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            client.Connect( remoteEP);  
            Thread.Sleep(1000); // async hack
            client.Receive(new byte[1024]); // retrieve and skip the INIT message
            Assert.AreEqual(1,server.NewConnectionsCount);
            Assert.AreEqual(0,server.ConnectedClients.Length);

            string expectedID = "ThisIsAnIdentifier";
            byte[] expectedIdBytes = Encoding.ASCII.GetBytes(expectedID);
            // Then send the init values
            client.Send(PacketProtocol.WrapMessage(MessageType.Init,expectedIdBytes));
            Thread.Sleep(1000); // async hack

            Assert.AreEqual(0,server.NewConnectionsCount);
            Assert.AreEqual(1,server.ConnectedClients.Length);
            Assert.AreEqual(expectedID,server.ConnectedClients[0]);
            server.Dispose();
        }

        [Test]
        [Ignore("a")]

        public void Dispose_ServerHasClient_ClientCantSendMessage()
        {
            Server server = new Server("localhost",20055);
            server.Start();

            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);  

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20055);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
  
            // Connect to the remote endpoint.  
            client.Connect( remoteEP);  
              
            // Send the data through the socket.  
            int bytesSent = client.Send(PacketProtocol.WrapMessage(MessageType.Standard,new byte[0]));   

            Thread.Sleep(1000); // async hack
            server.Dispose();
            Thread.Sleep(1000); // async hack
            client.Send(PacketProtocol.WrapKeepaliveMessage()); // send keepalive messages.
            SocketException exception  = Assert.Throws<SocketException>(() => client.Send(PacketProtocol.WrapKeepaliveMessage()));
            Assert.True(SocketError.Shutdown == exception.SocketErrorCode || SocketError.ConnectionAborted == exception.SocketErrorCode);
        }

        [Test]
        [Ignore("a")]

        public void Dispose_NewClientBeforeAndNewClientAfterDispose_NewClientAfterDisposeCantConnect()
        {
            Server server = new Server("localhost",20055);
            server.Start();

            // Establish the local endpoint for the socket.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());  
            IPAddress ipAddress = ipHostInfo.AddressList[0];  
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 20055);  

            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 20055);  
  
            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);  
            // Connect to the remote endpoint.  
            client.Connect( remoteEP);  
            server.Dispose();
            Thread.Sleep(1000); // async hack

            Socket client2 = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // I have to use the try/catch below as the exception that gets returned is from system.net.internals, which i cant cast to.
            try
            {
                client2.Connect(remoteEP);
                Assert.Fail("A connection was made after the server closed.");
            }
            catch(Exception e)
            {
                Assert.IsTrue(e.Message.Contains("Connection refused") || e.Message.Contains("No connection could be made because the target machine actively refused it"));
            }           
        }
    }
}