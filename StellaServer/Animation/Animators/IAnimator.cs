using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaServer.Animation.Animators
{
    /// <summary>
    /// An Animator creates a new frameset.
    /// </summary>
    public interface IAnimator
    {
         List<Frame> Create();
    }
}