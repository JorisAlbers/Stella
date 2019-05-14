using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Animation
{
    /// <summary>
    /// Duplicates the same animation over all pi's
    /// </summary>
    public class MirroringAnimator : IAnimator
    {
        private readonly IEnumerator<Frame>[] _frames;
        private readonly FrameSetMetadata _frameSetMetadata;

        public MirroringAnimator(IDrawer drawer, int numberOfPis, DateTime startAt)
        {
            _frames = new IEnumerator<Frame>[numberOfPis];
            for (int i = 0; i < numberOfPis; i++)
            {
                _frames[i] = drawer.GetEnumerator();
            }
            _frameSetMetadata = new FrameSetMetadata(startAt);
        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            if (piIndex > _frames.Length -1)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }

            _frames[piIndex].MoveNext();
            Frame frame = _frames[piIndex].Current;

            return frame;
        }


        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata(int piIndex)
        {
            return _frameSetMetadata;
        }
    }
}
