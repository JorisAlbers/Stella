﻿using System;
using System.Net;
using System.Net.Sockets;

namespace StellaLib.Network
{
    /// <summary>
    /// Interfaces the Socket functions
    /// </summary>
    public interface ISocketConnection
    {
        bool Connected { get; }

        EndPoint RemoteEndPoint { get; }

        IAsyncResult BeginReceive(byte[] buffer, int offset, int bufferSize, SocketFlags socketFlags,
            AsyncCallback callback, object state);

        IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, SocketFlags socketFlags,
            ref EndPoint remoteEP, AsyncCallback callback, object state);

        int EndReceive(IAsyncResult asyncResult);

        int EndReceiveFrom(IAsyncResult asyncResult, ref EndPoint endPoint);

        IAsyncResult BeginSend(byte[] buffer, int offset, int bufferSize, SocketFlags socketFlags, AsyncCallback callback, object state);

        int EndSend(IAsyncResult asyncResult);

        int SendTo(byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP);

        IAsyncResult BeginSendTo(byte[] buffer, int offset, int bufferSize, SocketFlags socketFlags,
            EndPoint remoteEndPoint, AsyncCallback callback,
            object state);


        void Bind(EndPoint endPoint);

        void Listen(int backlog);

        IAsyncResult BeginConnect(EndPoint endPoint, AsyncCallback callback, object state);

        void Connect(EndPoint endPoint);

        void EndConnect(IAsyncResult result);

        ISocketConnection EndAccept(IAsyncResult result);

        IAsyncResult BeginAccept(AsyncCallback callback, object state);

        void Shutdown(SocketShutdown how);

        void Disconnect(bool reuseSocket);

        void Dispose();

        void Close();

        bool EnableBroadcast { get; set; }
    }
}
