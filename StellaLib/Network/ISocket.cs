namespace StellaLib.Network
{
    public interface ISocket
    {
        void Send(byte[] dataToSend);
    }
}