using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using StellaClient.Light;
using StellaClient.Network;
using StellaLib.Animation;
using Timer = System.Timers.Timer;

namespace StellaClient
{
    public class RpiController
    {
        private readonly IStellaServer _server;
        private readonly ILedController _ledController;
        private const int FRAME_RECEIVED_THROTTLE_INTERVAL = 500;
        private Timer _frameReceivedThrottleTimer;
        private int _framesReceivedSinceLastInterval;
        private List<Frame> _receivedFramesBuffer;

        public RpiController(IStellaServer server, ILedController ledController)
        {
            _server = server;
            _ledController = ledController;

            // Frame Received Throttle stuff
            _frameReceivedThrottleTimer = new Timer(FRAME_RECEIVED_THROTTLE_INTERVAL);
            _frameReceivedThrottleTimer.AutoReset = true;
            _frameReceivedThrottleTimer.Elapsed += OnIntervalEnd;

            _server.AnimationStartReceived += ServerOnAnimationStartReceived;
            _server.FrameReceived += ServerOnFrameReceived;
            _ledController.FramesNeeded += LedControllerOnFramesNeeded;

        }

        private void ServerOnFrameReceived(object sender, Frame frame)
        {
            // We throttle the frames to reduce stress on the LedController
            if (!_frameReceivedThrottleTimer.Enabled)
            {
                _receivedFramesBuffer = new List<Frame>();
                _frameReceivedThrottleTimer.Start();
                _framesReceivedSinceLastInterval = 1;
            }
            else
            {
                _framesReceivedSinceLastInterval++;
            }
            _receivedFramesBuffer.Add(frame);
        }

        /// <summary>
        /// Called when the FrameReceivedThrottleTimer completed a loop.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnIntervalEnd(object sender, ElapsedEventArgs e)
        {
            if (_framesReceivedSinceLastInterval == _receivedFramesBuffer.Count || _receivedFramesBuffer.Count == 50)
            {
                // No frames have been added. Push to the LedController
                _ledController.AddFrames(_receivedFramesBuffer.OrderBy(x => x.Index));
                _receivedFramesBuffer = null;
                _frameReceivedThrottleTimer.Stop();
            }
        }

        private void LedControllerOnFramesNeeded(object sender, FramesNeededEventArgs e)
        {
            if (_frameReceivedThrottleTimer.Enabled)
            {
                // We are currently receiving frames. Ignore request for more frames.
                return;
            }
            // Notify the server that we need more frames
            // Add 1 to the lastFrameIndex to get the start index of the range we need
            _server.SendFrameRequest(e.LastFrameIndex+1, e.Count);
        }

        private void ServerOnAnimationStartReceived(object sender, FrameSetMetadata e)
        {
            // Notify the LedController that the server wants to start a new animation.
            _ledController.PrepareNextFrameSet(e);
        }
    }
}
