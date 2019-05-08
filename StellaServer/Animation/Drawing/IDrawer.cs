using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// An Animator creates a new frameset.
    /// </summary>
    public interface IDrawer
    {
         List<Frame> Create();
    }
}