using System.Net.Sockets;

namespace StellaLib.Network
{
    /// <summary>
    /// Represents a connection with a PI.
    /// Is a wrapper around a socket.
    /// </summary>
    public class PIClient : ISocket
    {
        private readonly Socket _socket;

        public PIClient(Socket socket)
        {
            _socket = socket;
        }

        public virtual void Send(byte[] data)
        {
            _socket.Send(data);
        }
    }
}