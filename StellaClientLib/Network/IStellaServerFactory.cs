using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace StellaClientLib.Network
{
    public interface IStellaServerFactory
    {
        IStellaServer Create(IPEndPoint serverAdress, int udpPort, int ID);
    }
}
