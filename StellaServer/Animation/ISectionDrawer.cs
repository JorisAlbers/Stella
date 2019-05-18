using System;
using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaServer.Animation
{
    public interface ISectionDrawer : IEnumerable<Frame>
    {
        /// <summary>
        /// The start time at which this combined drawer will start.
        /// </summary>
        DateTime Timestamp { get; }

    }
}