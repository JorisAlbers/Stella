using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using StellaClientLib.Network;

namespace StellaTestSuite.Model.Client
{
    public class MemoryStellaServerFactory : IStellaServerFactory
    {
        public IStellaServer Create(IPEndPoint serverAdress, int udpPort, int ID)
        {
            return new MemoryStellaServer();
        }
    }
}
