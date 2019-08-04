using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using rpi_ws281x;
using StellaClientLib.Light;
using StellaClientLib.Network;
using StellaClientLib.Serialization;

namespace StellaClientLib
{
    public class StellaClient : IDisposable
    {
        private static Configuration _configuration;
        private static LedController _ledController;
        private static IStellaServer _stellaServer;

        public StellaClient(Configuration configuration)
        {
            _configuration = configuration;
        }

        public void Start()
        {
            // Start the ledController
            try
            {
                Settings settings = new Settings(800000, _configuration.DmaChannel);
                settings.Channels[0] = new Channel(_configuration.LedCount, _configuration.PwmPin, _configuration.Brightness, false,
                    StripType.WS2812_STRIP);
                _ledController = new LedController(new WS281x(settings), _configuration.MinimumFrameRate);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to start the ledController (Are you running with sudo rights?)");
                throw;
            }

            // Start the StellaServer connection
            try
            {
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(_configuration.Ip), _configuration.Port);
                _stellaServer = new StellaServer(localEndPoint, _configuration.UdpPort, _configuration.Id);
                _stellaServer.RenderFrameReceived += (sender, frame) => _ledController.RenderFrame(frame);

                _stellaServer.Start();
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
