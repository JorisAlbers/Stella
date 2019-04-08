using System;
using System.Collections.Generic;
using System.Text;

namespace StellaClient.Light
{
    public class FramesNeededEventArgs : EventArgs
    {
        /// <summary> The last known frame index. -1 if unknown. </summary>
        public int LastFrameIndex { get; }
        /// <summary> The number of frames requested. </summary>
        public int Count { get; }

        public FramesNeededEventArgs(int lastFrameIndex, int count)
        {
            LastFrameIndex = lastFrameIndex;
            Count = count;
        }
    }
}
