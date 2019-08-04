using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaClientLib;
using StellaClientLib.Serialization;
using StellaTestSuite.Model.Client;

namespace StellaTestSuite
{
    class StellaTestSuite
    {
        private StellaClient[] _clients;
        private MemoryStellaServer[] _memoryStellaServers;

        public StellaTestSuite(int numberOfClients, int numberOfPixels, int minimumFrameRate, byte brightness)
        {
            _clients = new StellaClient[numberOfClients];
            _memoryStellaServers = new MemoryStellaServer[numberOfClients];
            for (int i = 0; i < numberOfClients; i++)
            {
                _memoryStellaServers[i] = new MemoryStellaServer();
                _clients[i] = new StellaClient(new Configuration(i, string.Empty, 0, 0, numberOfPixels, 0, 0, minimumFrameRate, brightness), _memoryStellaServers[i] );
            }
        }
    }
}
