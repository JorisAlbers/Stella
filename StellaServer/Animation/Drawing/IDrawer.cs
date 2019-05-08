using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// A Drawer creates a list of frames
    /// </summary>
    public interface IDrawer
    {
         List<Frame> Create();
    }
}