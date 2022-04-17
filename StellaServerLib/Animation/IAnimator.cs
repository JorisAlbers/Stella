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
        /// <summary>
        /// True if there is a next frame.
        /// Will try to calculate the next frame when the FrameMetaData is null.
        /// </summary>
        bool TryPeek(ref FrameMetadata previous);

        StoryboardTransformationController StoryboardTransformationController { get; }

        bool TryGetFramePerClient(out FrameWithoutDelta[] frames);

        void StartAnimation(int tickCount);
    }
}
