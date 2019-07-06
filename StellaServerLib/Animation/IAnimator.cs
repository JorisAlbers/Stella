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

        /// <summary> Get the brightness correction of a certain animation </summary>
        /// <param name="animationIndex"></param>
        float GetBrightnessCorrection(int animationIndex);

        /// <summary> Change the brightness of all leds by setting the brightness correction </summary>
        /// <param name="brightnessCorrection"></param>
        void SetBrightnessCorrection(float brightnessCorrection);

        /// <summary>
        /// Fade the color of all animations by setting the transformation factors.
        /// index 0 = red, 1 = green, 2 = blue.
        /// </summary>
        void SetRgbFadeCorrection( float[] transformationFactors);

        /// <summary>
        /// Fade the color of a specific animation by setting the transformation factors.
        /// index 0 = red, 1 = green, 2 = blue.
        /// </summary>
        void SetRgbFadeCorrection(int animationIndex, float[] transformationFactors);

        /// <summary>
        /// Get the RgbFadeCorrection of a certain animation
        /// </summary>
        float[] GetRgbFadeCorrection(int animationIndex);

    }
}
