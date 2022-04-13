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
        /// </summary>
        bool TryPeek(out int frameIndex, out long timeStampRelative);

        /// <summary>
        /// Only consumes if the inserted frameIndex and timestampRelative is equal to the frame prepared by the animator.
        /// True if it was consumed.
        /// False if it was not consumed.
        /// </summary>
        /// <returns></returns>
        public bool TryConsume(int frameIndex, long timestampRelative, out FrameWithoutDelta[] frames);


        StoryboardTransformationController StoryboardTransformationController { get; }

        /// <summary>
        /// Fired when the next storyboard starts an the startAt time needs to be reset. // TODO should no be needed, put the startAt inside the Animator
        /// </summary>
        event EventHandler TimeResetRequested;
    }
}
