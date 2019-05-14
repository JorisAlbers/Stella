using System;
using System.Collections.Generic;
using System.Diagnostics;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Animation
{
    /// <summary> Creates an unique animation on all pi's </summary>
    public class UniqueAnimator : IAnimator
    {
        private readonly IEnumerator<Frame>[] _frameEnumerators;
        private readonly FrameSetMetadata[] _frameSetMetadataPerPi;


        public UniqueAnimator(IDrawer[] drawersPerPi, DateTime[] startAtPerPi)
        {
            if (drawersPerPi.Length != startAtPerPi.Length)
            {
                throw new ArgumentException($"{nameof(drawersPerPi)} & {nameof(startAtPerPi)} must be of the same length");
            }

            _frameSetMetadataPerPi = new FrameSetMetadata[drawersPerPi.Length];
            _frameEnumerators = new IEnumerator<Frame>[drawersPerPi.Length];
            for (int i = 0; i < drawersPerPi.Length; i++)
            {
                _frameEnumerators[i] = drawersPerPi[i].GetEnumerator();
                _frameSetMetadataPerPi[i] = new FrameSetMetadata(startAtPerPi[i]);
            }

        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            if (piIndex > _frameEnumerators.Length - 1)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }

            _frameEnumerators[piIndex].MoveNext();
            Frame frame = _frameEnumerators[piIndex].Current;

            return frame;
        }

        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata(int piIndex)
        {
            if (piIndex > _frameEnumerators.Length - 1)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }

            return _frameSetMetadataPerPi[piIndex];
        }
    }
}
