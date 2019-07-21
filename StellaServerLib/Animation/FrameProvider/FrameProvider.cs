using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Animation.FrameProvider
{
    /// <summary>
    /// Provides the next frame to animate.
    /// </summary>
    public class FrameProvider : IFrameProvider
    {
        private readonly IDrawer _drawer;
        private readonly AnimationTransformation _animationTransformation;

        public FrameProvider(IDrawer drawer, AnimationTransformation animationTransformation)
        {
            _drawer = drawer;
            _animationTransformation = animationTransformation;
        }


        public IEnumerator<Frame> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
