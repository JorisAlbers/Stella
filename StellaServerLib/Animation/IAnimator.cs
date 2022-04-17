using System;
using System.Threading.Tasks;
using ReactiveUI;
using StellaLib.Animation;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    public interface IAnimator : IReactiveObject, IDisposable
    {
        StoryboardTransformationController StoryboardTransformationController { get; }

        bool TryGetFramePerClient(out FrameWithoutDelta[] frames);

        void StartAnimation(int tickCount);
    }
}
