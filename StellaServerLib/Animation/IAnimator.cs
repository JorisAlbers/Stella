using System;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    public interface IAnimator : IDisposable
    {
        /// <summary> Get the next frame, split over the pis</summary>
        /// <returns>A frame for each pi.</returns>
        bool TryGetNextFramePerPi(out FrameWithoutDelta[] frames);

        StoryboardTransformationController StoryboardTransformationController { get; }

        /// <summary>
        /// Fired when the next storyboard starts an the startAt time needs to be reset. // TODO should no be needed, put the startAt inside the Animator
        /// </summary>
        event EventHandler TimeResetRequested;
    }
}
