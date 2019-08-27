using StellaLib.Animation;

namespace StellaClientLib.Light
{
    public interface ILedController
    {
        void RenderFrame(FrameWithoutDelta frame);
    }
}