using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Network
{
    public class ServerFactory : IServerFactory
    {
        public IServer Create(string ip, int port, int udpPort)
        {
            return new Server(ip,port,udpPort);
        }
    }
}
