using System.Collections;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Animation.FrameProviding
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
            int timestampRelative = 0;
            int frameIndex = 0;
            IEnumerator<List<PixelInstruction>> drawerEnumerator = _drawer.GetEnumerator();

            while (true)
            {
                drawerEnumerator.MoveNext();
                Frame frame = new Frame(frameIndex, timestampRelative);
                frame.AddRange(drawerEnumerator.Current);
                
                yield return frame;
                timestampRelative += _animationTransformation.FrameWaitMs;
                frameIndex++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
