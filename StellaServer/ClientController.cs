using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
using StellaServer.Network;

namespace StellaServer
{
    public class ClientController
    {
        private readonly IServer _server;
        private FrameSet _frameSet;
        private object _frameSetLock = new object();
        public ClientController(IServer server)
        {
            _server = server;
            _server.AnimationRequestReceived += Server_OnAnimationRequestReceived;
        }
        
        // TODO add a Animation object. The animation object contains
        // TODO a FrameSet for each client, the id of the clients, etc
        public void StartAnimation(FrameSet frameSet)
        {
            // TODO for now, all clients get the same animation
            // Should be separately settable in the animation object.
            lock(_frameSetLock)
            {
                _frameSet = frameSet;
                
                // Send the ANIMATION_START message to all clients.
                // When the client has received the message and has prepared the animation buffer,
                // The client will request the first frames to fill it's buffer.
                byte[] message = FrameSetProtocol.Serialize(frameSet);
                foreach(string clientID in _server.ConnectedClients)
                {
                    _server.SendMessageToClient(clientID,MessageType.Animation_Start,message);
                }
            }
            
        }

        private void Server_OnAnimationRequestReceived(object sender, AnimationRequestEventArgs e)
        {
            lock(_frameSetLock)
            {
                if(_frameSet == null)
                {
                    Console.WriteLine($"Not sending frames to client {e.ClientID} as there is no frame set on display.");
                    return;
                }

                // Create packages
                List<byte[]> packages = new List<byte[]>();
                for (int i = e.StartIndex; i < e.StartIndex + e.Count; i++)
                {
                    if(i > _frameSet.Count)
                    {
                        break;
                    }

                    byte[][] bytes = FrameProtocol.SerializeFrame(_frameSet[i], PacketProtocol.MAX_MESSAGE_SIZE);
                    packages.AddRange(bytes);
                }

                // Send packages
                if(packages.Count < 1)
                {
                    Console.WriteLine($"Not sending frames to client {e.ClientID} as there are not any frames in the frameSet from {e.StartIndex} till {e.StartIndex + e.Count}.");
                    return;
                }

                foreach(byte[] packet in packages)
                {
                    _server.SendMessageToClient(e.ClientID,MessageType.Animation_Request,packet);
                }
            }
        }
    }
}