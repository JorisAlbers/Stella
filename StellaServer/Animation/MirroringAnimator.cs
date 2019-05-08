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

        public MirroringAnimator(IDrawer drawer, int numberOfPis)
        {
            _lastFramePerPi = new int[numberOfPis];
            _frames = drawer.Create();
        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            Debug.Assert(piIndex < _lastFramePerPi.Length, "The piIndex should not exceed the numberOfPis given on construction");

            int frameIndex = _lastFramePerPi[piIndex]++;
            return _frames[frameIndex];
        }
    }
}
