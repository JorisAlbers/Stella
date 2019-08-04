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
        private StellaClient[] clients;

        public StellaTestSuite(int numberOfClients, int numberOfPixelsPerRow, int numberOfRows, int minimumFrameRate, byte brightness)
        {
            clients = new StellaClient[numberOfClients];
            clients[0] = CreateClient(0,numberOfPixelsPerRow * numberOfRows, minimumFrameRate,brightness);
            clients[1] = CreateClient(1,numberOfPixelsPerRow * numberOfRows, minimumFrameRate,brightness);
            clients[2] = CreateClient(2,numberOfPixelsPerRow * numberOfRows, minimumFrameRate,brightness);
        }


        private StellaClient CreateClient(int id, int numberOfPixels, int minimumFrameRate, byte brightness)
        {
            Configuration configuration = new Configuration(id,string.Empty,0,0,numberOfPixels,0,0, minimumFrameRate, brightness);
            return new StellaClient(configuration, new MemoryStellaServerFactory());
        }
    }
}
