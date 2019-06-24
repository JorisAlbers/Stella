using System;
using System.Net;
using System.Net.Sockets;

namespace StellaLib.Network
{
    // Wrapper around Socket
    public class SocketConnection : ISocketConnection
    {
        private readonly Socket _socket;
        
        public SocketConnection(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
        {
            _socket = new Socket(addressFamily,socketType,protocolType);
        }

        public SocketConnection(SocketType socketType, ProtocolType protocolType)
        {
            _socket = new Socket(socketType, protocolType);
        }

        public SocketConnection(SocketInformation socketInformation)
        {
            _socket = new Socket(socketInformation);
        }
        
        public SocketConnection(Socket socket)
        {
            _socket = socket;
        }
        
        public bool Connected => _socket.Connected;

        public EndPoint RemoteEndPoint => _socket.RemoteEndPoint;

        public IAsyncResult BeginReceive(byte[] buffer, int offset, int bufferSize, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            return _socket.BeginReceive(buffer, offset, bufferSize, socketFlags, callback, state);
        }

        public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, object state)
        {
            return _socket.BeginReceiveFrom(buffer, offset, size, socketFlags, ref remoteEP, callback, state);
        }

        public int EndReceive(IAsyncResult asyncResult)
        {
            return _socket.EndReceive(asyncResult);
        }

        public IAsyncResult BeginSend(byte[] buffer, int offset, int bufferSize, SocketFlags socketFlags, AsyncCallback callback,
            object state)
        {
            return _socket.BeginSend(buffer, offset, bufferSize, socketFlags, callback, state);
        }

        public int EndSend(IAsyncResult asyncResult)
        {
            return _socket.EndSend(asyncResult);
        }

        public void Bind(EndPoint endPoint)
        {
            _socket.Bind(endPoint);
        }

        public void Listen(int backlog)
        {
            _socket.Listen(backlog);
        }

        public void Connect(EndPoint endPoint)
        {
            _socket.Connect(endPoint);
        }

        public IAsyncResult BeginConnect(EndPoint endPoint, AsyncCallback callback, object state)
        {
            return _socket.BeginConnect(endPoint, callback, state);
        }

        public void EndConnect(IAsyncResult result)
        {
            _socket.EndConnect(result);
        }


        public ISocketConnection EndAccept(IAsyncResult result)
        {
            return new SocketConnection(_socket.EndAccept(result));
        }

        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            return _socket.BeginAccept(callback, state);
        }

        public void Shutdown(SocketShutdown how)
        {
            _socket.Shutdown(how);
        }

        public void Disconnect(bool reuseSocket)
        {
            _socket.Disconnect(reuseSocket);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }

        public void Close()
        {
            _socket.Close();
        }
    }
}
