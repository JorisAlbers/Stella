using System;
using System.Collections.Generic;
using System.Text;
using StellaClient.Light;
using StellaClient.Network;
using StellaLib.Animation;

namespace StellaClient
{
    public class RpiController
    {
        private readonly IStellaServer _server;
        private readonly ILedController _ledController;

        public RpiController(IStellaServer server, ILedController ledController)
        {
            _server = server;
            _ledController = ledController;

            _server.AnimationStartReceived += ServerOnAnimationStartReceived;
            _ledController.FramesNeeded += LedControllerOnFramesNeeded;
        }

        private void LedControllerOnFramesNeeded(object sender, FramesNeededEventArgs e)
        {
            // Notify the server that we need more frames
            _server.SendFrameRequest(e.LastFrameIndex, e.Count);
        }

        private void ServerOnAnimationStartReceived(object sender, FrameSetMetadata e)
        {
            // Notify the LedController that the server wants to start a new animation.
            _ledController.PrepareNextFrameSet(e);
        }
    }
}
