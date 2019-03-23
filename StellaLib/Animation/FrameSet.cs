using System;
using System.Collections;
using System.Collections.Generic;

namespace StellaLib.Animation
{
    /// <summary>
    /// Collection of Frames.
    /// </summary>
    /// <typeparam name="Frame"></typeparam>
    public class FrameSet : List<Frame>
    {
        public FrameSetMetadata Metadata { get; private set; }

        public DateTime TimeStamp => Metadata.TimeStamp;

        public FrameSet(DateTime timestamp)
        {
            Metadata = new FrameSetMetadata(timestamp);
        }
    }
}