using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerAPI
{
    public enum MessageType
    {
        None,
        /// <summary> Get the names of the preloaded storyboard </summary>
        GetAvailableStoryboards,
        /// <summary> Start a preloaded storyboard (by providing the name) </summary>
        StartPreloadedStoryboard,
        /// <summary> Start a storyboard (by providing the yaml)</summary>
        StartStoryboard,
        /// <summary> Store a bitmap on StellaServer </summary>
        StoreBitmap,

    }
}
