﻿using StellaLib.Animation;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    public interface IAnimator
    {
        /// <summary> Get the next frame, split over the pis</summary>
        /// <returns>A frame for each pi.</returns>
        Frame[] GetNextFramePerPi();
    }

    public interface IAnimatorWithoutDelta
    {
        /// <summary> Get the next frame, split over the pis</summary>
        /// <returns>A frame for each pi.</returns>
        FrameWithoutDelta[] GetNextFramePerPi();
    }
}
