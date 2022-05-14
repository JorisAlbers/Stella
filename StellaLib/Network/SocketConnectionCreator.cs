using System.Net;
using System.Net.Sockets;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public class SocketConnectionCreator
    {
        public virtual ISocketConnection CreateForBroadcast(int port)
        {
            var localEp = new IPEndPoint(IPAddress.Any, port);
            SocketConnection socket = new SocketConnection(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.Bind(localEp);
            return socket;
        }

        public virtual ISocketConnection Create(IPEndPoint localEndPoint)
        {
            SocketConnection socket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = false;
            socket.Bind(localEndPoint);
            return socket;
        }
    }
}