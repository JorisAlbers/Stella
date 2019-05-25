﻿using System;
using System.Collections.Generic;
using System.Text;
using StellaLib.Animation;

namespace StellaServer.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    public interface IAnimator
    {
        /// <summary> Get the next frame, split over the pis</summary>
        /// <returns>A frame for each pi.</returns>
        Frame[] GetNextFramePerPi();

        /// <summary>
        /// Get the frame set metadata
        /// </summary>
        /// <returns></returns>
        FrameSetMetadata GetFrameSetMetadata();

    }
}
