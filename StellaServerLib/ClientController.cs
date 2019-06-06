using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServerLib
{
    public class ClientController
    {
        private readonly IServer _server;
        private IAnimator _animator;
        private object _frameSetLock = new object();

        private List<Frame[]> _framesPerPi;

        public ClientController(IServer server)
        {
            _server = server;
            _server.AnimationRequestReceived += Server_OnAnimationRequestReceived;
        }
        
        public void StartAnimation(IAnimator animator)
        {
            lock(_frameSetLock)
            {
                _animator = animator;

                // Create a buffer of frames per pi
                _framesPerPi = new List<Frame[]>();
                _framesPerPi.Add(_animator.GetNextFramePerPi());

                // Send the ANIMATION_START message to all clients.
                // When the client has received the message and has prepared the animation buffer,
                // The client will request the first frames to fill it's buffer.
                FrameSetMetadata frameSetMetadata = _animator.GetFrameSetMetadata();
                foreach(int clientID in _server.ConnectedClients)
                {
                    byte[] message = FrameSetMetadataProtocol.Serialize(frameSetMetadata);
                    _server.SendMessageToClient(clientID,MessageType.Animation_Start,message);
                }
            }
            
        }

        private void Server_OnAnimationRequestReceived(object sender, AnimationRequestEventArgs e)
        {
            lock(_frameSetLock)
            {
                // Get the frames requested
                if(_animator == null)
                {
                    Console.WriteLine($"Not sending frames to client {e.ClientID} as there is no animation playing.");
                    return;
                }

                // TODO remove buffer from pis and implement Real Time protocol.
                // TODO then this ClientController can decide when to send a new frame instead of the pis requesting new frames.
                int lastFrameIndex = _framesPerPi[_framesPerPi.Count-1][0].Index;
                int framesMissing = e.StartIndex + e.Count - lastFrameIndex;
                if (framesMissing > 0)
                {
                    // Generate new frames
                    for (int i = 0; i < framesMissing + 50; i++)
                    {
                        _framesPerPi.Add(_animator.GetNextFramePerPi());
                    }
                    // TODO remove frames to prevent memory overflow
                }

                List<Frame> frames = new List<Frame>();
                for (int i = 0; i < e.Count; i++)
                {
                    frames.Add(_framesPerPi[i + e.StartIndex][e.ClientID]);
                }
                   

                Console.Out.WriteLine($"Sending {frames.Count} frames to client {e.ClientID}");
                // Send the frames
                List<byte[]> packages = AnimationRequestProtocol.CreateResponse(frames);

                foreach(byte[] packet in packages)
                {
                    _server.SendMessageToClient(e.ClientID,MessageType.Animation_Request,packet);
                }
            }
        }
    }
}