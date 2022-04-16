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

        /// <summary>
        /// Fired when the next storyboard starts an the startAt time needs to be reset. // TODO should no be needed, put the startAt inside the Animator
        /// </summary>
        event EventHandler TimeResetRequested;
    }
}
