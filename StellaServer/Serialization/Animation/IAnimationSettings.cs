using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServer.Serialization.Animation
{
    /// <summary>
    /// Settings of an animation
    /// </summary>
    public interface IAnimationSettings
    {
        int StartIndex { get; set; }
        int StripLength { get; set; }
        int FrameWaitMs { get; set; }
    }
}
