using System;
using System.Collections.Generic;
using System.Text;
using StellaLib.Animation;

namespace StellaServer.Animation
{
    /// <summary>
    /// A animator decides what the next frame of a pi looks like. 
    /// </summary>
    interface IAnimator
    {
        /// <summary> Get the next frame for a pi at a certain index</summary>
        /// <param name="piIndex"></param>
        /// <returns></returns>
        Frame GetNextFrame(int piIndex);

        /// <summary>
        /// Get the frame set metadata for a pi at a certain index
        /// </summary>
        /// <param name="piIndex"></param>
        /// <returns></returns>
        FrameSetMetadata GetFrameSetMetadata(int piIndex);

    }
}
