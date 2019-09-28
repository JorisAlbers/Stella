using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation.FrameProviding
{
    /// <summary>
    /// Provides the next frame to animate.
    /// </summary>
    public class FrameProvider : IFrameProvider
    {
        private readonly IDrawer[] _drawers;
        private readonly TransformationController _transformationController;
        private readonly int _timeUnitMs;
        private readonly int[] _relativeStartingTimestamps;
        private readonly int _firstTimestamp;

        private int[] _timestamps;
        private int   _frameIndex;

        /// <summary>
        /// Create a new FrameProvider with a single drawer
        /// </summary>
        /// <param name="drawer"></param>
        /// <param name="transformationController"></param>
        /// <param name="timeUnitMs">The number of milliseconds each time unit takes.</param>
        public FrameProvider(IDrawer drawer, TransformationController transformationController, int timeUnitMs)
        {
            _drawers = new[] { drawer };
            _transformationController = transformationController;
            _timeUnitMs = timeUnitMs;
            _relativeStartingTimestamps = new []{0};
            _timestamps = new int[1];
            _drawers[0].MoveNext();
        }

        /// <summary>
        /// Create a new FrameProvider with multiple drawers
        /// </summary>
        /// <param name="drawers"></param>
        /// <param name="relativeStartingTimestamps">The time to start each frame relative to each other, in milliseconds</param>
        /// <param name="transformationController"></param>
        public FrameProvider(IDrawer[] drawers, int[] relativeStartingTimestamps, TransformationController transformationController, int timeUnitMs)
        {
            _drawers = drawers;
            _transformationController = transformationController;
            _relativeStartingTimestamps = relativeStartingTimestamps;
            _firstTimestamp = relativeStartingTimestamps.Min();
            _timeUnitMs = timeUnitMs;
            _timestamps = new int[drawers.Length];

            // Initialize frames
            for (int i = 0; i < _drawers.Length; i++)
            {
                _drawers[i].MoveNext();
            }
        }
        
        /// <summary>
        /// Returns the indexes of the drawers that have a frame starting before the other drawers.
        /// </summary>
        /// <returns></returns>
        private List<int> GetNextInLineAnimations(int[] timestamps)
        {
            int firstTimestamp = int.MaxValue;
            List<int> sectionIndexes = null;
            for (int i = 0; i < timestamps.Length; i++)
            {
                int startAt = _relativeStartingTimestamps[i] + timestamps[i];
                if (startAt < firstTimestamp)
                {
                    firstTimestamp = startAt;
                    sectionIndexes = new List<int>() { i };
                    continue;
                }

                if (startAt == firstTimestamp)
                {
                    Debug.Assert(sectionIndexes != null, nameof(sectionIndexes) + " != null");
                    sectionIndexes.Add(i);
                }
            }

            return sectionIndexes;
        }

        public bool MoveNext()
        {
            // Get the AnimationTransformation
            AnimationTransformation animationTransformation = _transformationController.AnimationTransformation;

            // Find the section that will start first
            List<int> providersInNextFrame = GetNextInLineAnimations(_timestamps);
            if (providersInNextFrame.Count == 0)
            {
                throw new Exception("There must always be a next frame");
            }

            int firstIndex = providersInNextFrame[0];

            // Calculate the timestampRelative
            int timestampFirstDrawer = _relativeStartingTimestamps[providersInNextFrame[0]] + _timestamps[firstIndex];
            int deltaWithOverallTimestamp = timestampFirstDrawer - _firstTimestamp;
            Frame frame = new Frame(_frameIndex, deltaWithOverallTimestamp);

            // Add the frames of each drawer
            foreach (int providerIndex in providersInNextFrame)
            {
                List<PixelInstructionWithDelta> instructions = _drawers[providerIndex].Current;
                for (int j = 0; j < instructions.Count; j++)
                {
                    PixelInstructionWithDelta pixelInstructionWithDelta = instructions[j];
                    (byte red, byte green, byte blue) = animationTransformation.AdjustColor(providerIndex, pixelInstructionWithDelta.R,
                        pixelInstructionWithDelta.G, pixelInstructionWithDelta.B);
                    pixelInstructionWithDelta.R = red;
                    pixelInstructionWithDelta.G = green;
                    pixelInstructionWithDelta.B = blue;
                    frame.Add(pixelInstructionWithDelta);
                }
            }

            Current = frame;

            // Get the next frames of the used drawers
            foreach (int sectionIndex in providersInNextFrame)
            {
                _timestamps[sectionIndex] += animationTransformation.GetCorrectedTimeUnitsPerFrame(sectionIndex) * _timeUnitMs;
                _drawers[sectionIndex].MoveNext();
            }

            // Prepare for the next round
            _frameIndex++;
            return true;
        }

        public void Reset()
        {
            foreach (IDrawer drawer in _drawers)
            {
                drawer.Reset();
            }

            _frameIndex = 0;
            _timestamps = new int[_drawers.Length];
        }

        public Frame Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
