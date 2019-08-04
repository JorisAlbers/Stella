using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StellaClientLib.Network
{
    public class StellaServerFactory : IStellaServerFactory
    {
        public IStellaServer Create(IPEndPoint serverAdress, int udpPort, int ID)
        {
            return new StellaServer(serverAdress, udpPort, ID);
        }
    }
}
