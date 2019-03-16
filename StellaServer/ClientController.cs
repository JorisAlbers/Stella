using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol.Animation;
using StellaServer.Network;

namespace StellaServer
{
    public class ClientController
    {
        private readonly IServer _server;
        private FrameSet _currentlyPlayingFrameSet;
        public ClientController(IServer server)
        {
            _server = server;
        }
        
        // TODO add a Animation object. The animation object contains
        // TODO a FrameSet for each client, the id of the clients, etc
        public void StartAnimation(FrameSet frameSet)
        {
            // TODO for now, all clients get the same animation
            // Should be separately settable in the animation object.
            _currentlyPlayingFrameSet = frameSet;
            
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
}