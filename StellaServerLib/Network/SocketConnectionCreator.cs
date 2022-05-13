using System.Net;
using System.Net.Sockets;
using StellaLib.Network;

namespace StellaServerLib.Network
{
    public class SocketConnectionCreator
    {
        public ISocketConnection CreateForBroadcast(IPEndPoint localEndPoint)
        {
            SocketConnection socket = new SocketConnection(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;
            socket.Bind(localEndPoint);
            return socket;
        }
    }
}