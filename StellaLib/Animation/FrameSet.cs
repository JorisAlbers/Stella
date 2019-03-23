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
        public DateTime TimeStamp {get;private set;}

        public FrameSet(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }
    }
}