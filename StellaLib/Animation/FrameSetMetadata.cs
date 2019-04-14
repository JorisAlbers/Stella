using System;
using System.Collections.Generic;
using System.Text;

namespace StellaLib.Animation
{
    // Class that only contains metadata off a FrameSet. 
    // This class is used to send the initial data of a frameSet from the server
    // to the client. 
    public class FrameSetMetadata
    {
        public DateTime TimeStamp { get; private set; }

        public FrameSetMetadata(DateTime timeStamp)
        {
            TimeStamp = timeStamp;
        }
    }
}
