using StellaLib.Network;

namespace StellaClient.Network
{
    public interface IStellaServer
    {
        void Dispose();
        void Send(MessageType type, byte[] message);
        void Start();
    }
}