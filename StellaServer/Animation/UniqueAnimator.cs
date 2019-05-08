using System.Collections.Generic;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Animation
{
    /// <summary> Creates an unique animation on all pi's </summary>
    public class UniqueAnimator : IAnimator
    {
        private readonly List<Frame>[] _framesPerPi;
        private readonly int[] _lastFramePerPi;


        public UniqueAnimator(IDrawer[] drawersPerPi)
        {
            _lastFramePerPi = new int[drawersPerPi.Length];
            _framesPerPi = new List<Frame>[drawersPerPi.Length];
            for (int i = 0; i < drawersPerPi.Length; i++)
            {
                _framesPerPi[i] = drawersPerPi[i].Create();
            }
        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            int frameIndex = _lastFramePerPi[piIndex]++;
            return _framesPerPi[piIndex][frameIndex];
        }
    }
}
