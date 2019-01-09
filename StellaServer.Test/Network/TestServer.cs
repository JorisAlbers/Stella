using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using StellaLib.Network;
using StellaServer.Network;

namespace StellaServer.Test.Animation.Network
{
    [TestFixture]
    public class TestServer
    {
        [Test]
        public void SendDataToClient_DefaultMessage_MessageSent()
        {
            Server server = new Server(20055);
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

            string ID = "ThisIsAnIdentifier";
            byte[] message = Encoding.ASCII.GetBytes($"{MessageType.Init.ToString()};{ID}<EOF>");
            // Then send the init values
            client.Send(message);
            Thread.Sleep(1000); // async hack

            string expectedData = "ThisIsAMessage";
            string expectedMessage = $"{MessageType.Standard};{expectedData}<EOF>";
            server.SendMessageToClient(ID,expectedData);

            byte[] buffer = new byte[1024];
            int bytesRead = client.Receive(buffer);

            Assert.AreEqual(expectedMessage,Encoding.ASCII.GetString(buffer, 0, bytesRead));
            server.Dispose();
        }


        [Test]
        public void InitMessageSent_ExistingClient_ClientReplaced()
        {
            Server server = new Server(20055);
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

            string expectedID = "ThisIsAnIdentifier";
            byte[] message = Encoding.ASCII.GetBytes($"{MessageType.Init.ToString()};{expectedID}<EOF>");
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


        [Test]
        public void InitMessageSent_NewClient_ClientGetsMovedToListOfClients()
        {
            Server server = new Server(20055);
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
            Assert.AreEqual(1,server.NewConnectionsCount);
            Assert.AreEqual(0,server.ConnectedClients.Length);

            string expectedID = "ThisIsAnIdentifier";
            // Then send the init values
            client.Send(Encoding.ASCII.GetBytes($"{MessageType.Init.ToString()};{expectedID}<EOF>"));
            Thread.Sleep(1000); // async hack

            Assert.AreEqual(0,server.NewConnectionsCount);
            Assert.AreEqual(1,server.ConnectedClients.Length);
            Assert.AreEqual(expectedID,server.ConnectedClients[0]);
            server.Dispose();
        }

        [Test]
        public void Dispose_ServerHasClient_ClientCantSendMessage()
        {
            Server server = new Server(20055);
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

            // Encode the data string into a byte array.  
            byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");  
  
            // Send the data through the socket.  
            int bytesSent = client.Send(msg);   

            Thread.Sleep(1000); // async hack
            server.Dispose();
            Thread.Sleep(1000); // async hack
            client.Send(msg); // why do we need to send a message first? maybe add keepalive messages.
            SocketException exception  = Assert.Throws<SocketException>(() => client.Send(Encoding.ASCII.GetBytes("This is a test<EOF>")));
            Assert.AreEqual(SocketError.Shutdown,exception.SocketErrorCode);
        }

        [Test]
        public void Dispose_NewClientBeforeAndNewClientAfterDispose_NewClientAfterDisposeCantConnect()
        {
            Server server = new Server(20055);
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
                Assert.IsTrue(e.Message.Contains("Connection refused"));
            }           
        }
    }
}