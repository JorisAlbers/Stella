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
        private readonly List<Frame>[] _framesPerPi;
        private readonly int[] _lastFramePerPi;
        private readonly FrameSetMetadata[] _frameSetMetadataPerPi;


        public UniqueAnimator(IDrawer[] drawersPerPi, DateTime[] startAtPerPi)
        {
            if (drawersPerPi.Length != startAtPerPi.Length)
            {
                throw new ArgumentException($"{nameof(drawersPerPi)} & {nameof(startAtPerPi)} must be of the same length");
            }

            _lastFramePerPi = new int[drawersPerPi.Length];
            _framesPerPi = new List<Frame>[drawersPerPi.Length];
            _frameSetMetadataPerPi = new FrameSetMetadata[drawersPerPi.Length];
            for (int i = 0; i < drawersPerPi.Length; i++)
            {
                _framesPerPi[i] = drawersPerPi[i].Create();
                _frameSetMetadataPerPi[i] = new FrameSetMetadata(startAtPerPi[i]);
            }

        }

        /// <inheritdoc />
        public Frame GetNextFrame(int piIndex)
        {
            if (piIndex > _lastFramePerPi.Length)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }
            
            int frameIndex = _lastFramePerPi[piIndex];
            Frame frame = _framesPerPi[piIndex][frameIndex];
            _lastFramePerPi[piIndex] = ++frameIndex % _framesPerPi[piIndex].Count;

            return frame;
        }

        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata(int piIndex)
        {
            if (piIndex > _lastFramePerPi.Length)
            {
                throw new ArgumentException($"The {nameof(piIndex)} should not exceed the numberOfPis given on construction");
            }

            return _frameSetMetadataPerPi[piIndex];
        }
    }
}
