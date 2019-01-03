using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using NUnit.Framework;
using StellaLib.Network;

namespace StellaLib.Test.Animation.Network
{
    [TestFixture]
    public class TestServer
    {
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