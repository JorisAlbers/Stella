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
        /// <summary> Start a storyboard </summary>
        StartPreloadedStoryboard,
        /// <summary> Store a bitmap on StellaServer </summary>
        StoreBitmap
    }
}
