using StellaLib.Animation;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    public interface IAnimator
    {
        /// <summary> Get the next frame, split over the pis</summary>
        /// <returns>A frame for each pi.</returns>
        FrameWithoutDelta[] GetNextFramePerPi();

        /// <summary>
        /// Set the frame wait time for all animations.
        /// </summary>
        void SetFrameWaitMs(int frameWaitMs);

        /// <summary>
        /// Set the frame wait time for a specific animation
        /// </summary>
        void SetFrameWaitMs(int frameWaitMs, int animationIndex);

        /// <summary>
        /// Gets the current frameWaitMs of the animation at the specified index.
        /// </summary>
        int GetFrameWaitMs(int animationIndex);

        /// <summary> Change the brightness of all leds by setting the brightness correction </summary>
        /// <param name="brightnessCorrection"></param>
        void SetBrightnessCorrection(float brightnessCorrection);

    }
}
