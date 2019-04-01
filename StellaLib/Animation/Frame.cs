using System.Collections;
using System.Collections.Generic;

namespace StellaLib.Animation
{
    /// <summary>
    /// A frame contains:
    ///     1. All pixel instructions of a single moment in time.
    ///     2. The display time 
    /// 
    /// </summary>
    public class Frame : List<PixelInstruction>
    {   
        /// <summary>
        /// The index of the Frame in the frameSet.
        /// </summary>
        public int Index;

        /// <summary>
        /// The number of milliseconds after the start of the FrameSet (animation) this frame will be shown.
        /// </summary>
        /// <value></value>
        public int TimeStampRelative {get;private set;}

        public Frame(int index, int timeStampRelative)
        {
            Index = index;
            TimeStampRelative = timeStampRelative;
        }
    }
}