using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace StellaLib.Animation
{
    /// <summary>
    /// A frame contains:
    ///     1. All pixel instructions of a single moment in time.
    ///     2. The display time 
    /// 
    /// </summary>
    [DebuggerDisplay("Index = {Index}, TSR = {TimeStampRelative}, Count = {Count}")]
    public class Frame : List<PixelInstruction> , IEquatable<Frame>
    {   
        /// <summary>
        /// The index of the Frame in the frameSet.
        /// </summary>
        public int Index;

        /// <summary>
        /// The number of milliseconds after the start of the FrameSet (animation) this frame will be shown.
        /// </summary>
        /// <value></value>
        public int TimeStampRelative {get; set;}

        public Frame(int index, int timeStampRelative)
        {
            Index = index;
            TimeStampRelative = timeStampRelative;
        }

        public bool Equals(Frame other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Index != other.Index) return false;
            if (TimeStampRelative != other.TimeStampRelative) return false;
            if (Count != other.Count) return false;
            for (int i = 0; i < Count; i++)
            {
                if (!this[i].Equals(other[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}