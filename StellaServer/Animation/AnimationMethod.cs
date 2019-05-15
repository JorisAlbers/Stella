using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServer.Animation
{
    /// <summary> The method at which to animate </summary>
    public enum AnimationMethod
    {
        /// <summary> Not set </summary>
        Unknown,

        /// <summary> Duplicate the same animation on all pi's </summary>
        Mirror,

        /// <summary> Create an unique animation on all pi's </summary>
        Unique,

    }
}
