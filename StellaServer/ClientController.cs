using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
using StellaServer.Animation;
using StellaServer.Network;

namespace StellaServer
{
    public class ClientController
    {
        private readonly IServer _server;
        private IAnimator _animator;
        private object _frameSetLock = new object();
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
                // Send the ANIMATION_START message to all clients.
                // When the client has received the message and has prepared the animation buffer,
                // The client will request the first frames to fill it's buffer.

                foreach(int clientID in _server.ConnectedClients)
                {
                    byte[] message = FrameSetMetadataProtocol.Serialize(_animator.GetFrameSetMetadata(clientID));
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

                List<Frame> frames = new List<Frame>();
                for (int i = 0; i < e.Count; i++)
                {
                    frames.Add(_animator.GetNextFrame(e.ClientID));
                }

                if(frames.Count < 1)
                {
                    Console.WriteLine($"Not sending frames to client {e.ClientID} as there are not any frames in the frameSet from {e.StartIndex} till {e.StartIndex + e.Count}.");
                    return;
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