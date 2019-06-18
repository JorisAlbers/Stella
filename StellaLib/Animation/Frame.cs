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


        [DebuggerStepThrough]
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

    /// <summary>
    /// Frame with pixelInstructions without a delta, so no indexes.
    /// A frame contains:
    ///     1. All pixel instructions of a single moment in time.
    ///     2. The display time 
    /// 
    /// </summary>
    [DebuggerDisplay("Index = {Index}, TSR = {TimeStampRelative}, Count = {Items.Length}")]
    public class FrameWithoutDelta : IEquatable<FrameWithoutDelta>
    {
        /// <summary>
        /// The index of the Frame in the frameSet.
        /// </summary>
        public int Index;

        /// <summary>
        /// The number of milliseconds after the start of the FrameSet (animation) this frame will be shown.
        /// </summary>
        /// <value></value>
        public int TimeStampRelative { get; set; }

        /// <summary>
        /// The pixelInstructions in this list.
        /// </summary>
        public PixelInstructionWithoutDelta[] Items { get; }

        /// <summary>
        /// The number of pixelInstructions in this list
        /// </summary>
        public int Count => Items.Length;

        public PixelInstructionWithoutDelta this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }


        [DebuggerStepThrough]
        public FrameWithoutDelta(int index, int timeStampRelative, int capacity)
        {
            Index = index;
            TimeStampRelative = timeStampRelative;
            Items = new PixelInstructionWithoutDelta[capacity];
        }

        public FrameWithoutDelta(int index, int timeStampRelative, PixelInstructionWithoutDelta[] items)
        {
            Index = index;
            TimeStampRelative = timeStampRelative;
            Items = items;
        }

        public bool Equals(FrameWithoutDelta other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Index != other.Index) return false;
            if (TimeStampRelative != other.TimeStampRelative) return false;
            if (Items.Length != other.Items.Length) return false;
            for (int i = 0; i < Items.Length; i++)
            {
                if (!this.Items[i].Equals(other.Items[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}