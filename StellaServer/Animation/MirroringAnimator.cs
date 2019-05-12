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
        private readonly List<Frame> _frames;
        private readonly int[] _lastFramePerPi;
        private readonly FrameSetMetadata _frameSetMetadata;

        public MirroringAnimator(IDrawer drawer, int numberOfPis, DateTime startAt)
        {
            _lastFramePerPi = new int[numberOfPis];
            _frames = drawer.Create();
            _frameSetMetadata = new FrameSetMetadata(startAt);
        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            if (piIndex > _lastFramePerPi.Length)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }

            int frameIndex = _lastFramePerPi[piIndex];
            Frame frame = _frames[frameIndex];
            _lastFramePerPi[piIndex] = ++frameIndex % _frames.Count;

            return frame;
        }


        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata(int piIndex)
        {
            return _frameSetMetadata;
        }
    }
}
