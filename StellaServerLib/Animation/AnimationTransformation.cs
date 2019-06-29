using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation
{
    // Used to change animation characteristics during animation
    class AnimationTransformation
    {
        public int FrameWaitMs { get; set; }

        public AnimationTransformation(int initialFrameWaitMs)
        {
            FrameWaitMs = initialFrameWaitMs;
        }
    }
}
