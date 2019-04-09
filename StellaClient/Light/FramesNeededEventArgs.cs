using System;
using System.Collections.Generic;
using System.Text;

namespace StellaClient.Light
{
    public class FramesNeededEventArgs : EventArgs
    {
        /// <summary> The first frame index requested. </summary>
        public int FromIndex { get; }
        /// <summary> The number of frames requested. </summary>
        public int Count { get; }

        public FramesNeededEventArgs(int fromIndex, int count)
        {
            FromIndex = fromIndex;
            Count = count;
        }
    }
}
