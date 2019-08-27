using System;
using System.Net;
using rpi_ws281x;
using StellaClientLib.Light;
using StellaClientLib.Network;
using StellaClientLib.Serialization;

namespace StellaClientLib
{
    public class StellaClient : IDisposable
    {
        private readonly Configuration _configuration;
        private LedController _ledController;
        private readonly IStellaServer _stellaServer;
        private readonly ILEDStrip _ledStrip;

        public StellaClient(Configuration configuration, IStellaServer stellaServer, ILEDStrip ledStrip)
        {
            _configuration = configuration;
            _stellaServer = stellaServer;
            _ledStrip = ledStrip;
        }

        public void Start()
        {
            // Start the ledController
            try
            {
                _ledController = new LedController(_ledStrip, _configuration.MinimumFrameRate);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start the ledController (Are you running with sudo rights?)");
                throw;
            }

            // Start the StellaServer connection
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(_configuration.Ip), _configuration.Port);
                _stellaServer.RenderFrameReceived += (sender, frame) => _ledController.RenderFrame(frame);
                _stellaServer.Start(remoteEndPoint, _configuration.UdpPort, _configuration.Id);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start opening a connection with the server.");
                throw;
            }
        }


        public void Dispose()
        {
            _stellaServer.Dispose();
        }
    }
}
