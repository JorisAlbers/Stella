namespace StellaLib.Network
{
    public enum MessageType
    {
        Unknown,
        Init,
        Standard,
        AnimationRenderFrame,
        FrameHeader,
        FrameSection,
        ConnectionRequest,
    }
}