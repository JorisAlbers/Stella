using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Network
{
    public interface IServerFactory
    {
        IServer Create(string ip, int port, int udpPort);
    }
}
