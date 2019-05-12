using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Assert(piIndex < _lastFramePerPi.Length, "The piIndex should not exceed the numberOfPis given on construction");
            
            int frameIndex = _lastFramePerPi[piIndex];
            Frame frame = _framesPerPi[piIndex][frameIndex];
            _lastFramePerPi[piIndex] = ++frameIndex % _framesPerPi[piIndex].Count;

            return frame;
        }
    }
}
