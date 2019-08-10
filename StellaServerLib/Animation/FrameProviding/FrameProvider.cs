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
        private readonly IEnumerator<List<PixelInstruction>>[] _framesPerDrawer;
        private readonly AnimationTransformations _animationTransformations;
        private readonly int[] _relativeStartingTimestamps;
        private readonly int _firstTimestamp;
        
        /// <summary>
        /// Create a new FrameProvider with a single drawer
        /// </summary>
        /// <param name="drawer"></param>
        /// <param name="animationTransformations></param>
        public FrameProvider(IDrawer drawer, AnimationTransformations animationTransformations)
        {
            _framesPerDrawer = new[] { drawer.GetEnumerator() };
            _animationTransformations = animationTransformations;
            _relativeStartingTimestamps = new []{0};
        }

        /// <summary>
        /// Create a new FrameProvider with multiple drawers
        /// </summary>
        /// <param name="drawers"></param>
        /// <param name="relativeStartingTimestamps">The time to start each frame relative to each other, in milliseconds</param>
        /// <param name="animationTransformations"></param>
        public FrameProvider(IDrawer[] drawers, int[] relativeStartingTimestamps,AnimationTransformations animationTransformations)
        {
            _framesPerDrawer = drawers.Select(x => x.GetEnumerator()).ToArray();
            _animationTransformations = animationTransformations;
            _relativeStartingTimestamps = relativeStartingTimestamps;
            _firstTimestamp = relativeStartingTimestamps.Min();
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int[] timestamps = new int[_framesPerDrawer.Length];
            int frameIndex = 0;

            // Initialize frames
            for (int i = 0; i < _framesPerDrawer.Length; i++)
            {
                _framesPerDrawer[i].MoveNext();
            }

            while (true)
            {
                // Find the section that will start first
                List<int> providersInNextFrame = GetNextInLineAnimations(timestamps);
                if (providersInNextFrame.Count == 0)
                {
                    throw new Exception("There must always be a next frame");
                }

                int firstIndex = providersInNextFrame[0];

                // Calculate the timestampRelative
                int timestampFirstDrawer = _relativeStartingTimestamps[providersInNextFrame[0]] + timestamps[firstIndex];
                int deltaWithOverallTimestamp = timestampFirstDrawer - _firstTimestamp;
                Frame frame = new Frame(frameIndex, deltaWithOverallTimestamp);

                // Add the frames of each drawer
                foreach (int providerIndex in providersInNextFrame)
                {
                    List<PixelInstruction> instructions = _framesPerDrawer[providerIndex].Current;
                    for (int j = 0; j < instructions.Count; j++)
                    {
                        PixelInstruction pixelInstruction = instructions[j];
                        pixelInstruction.Color = _animationTransformations.AdjustColor(providerIndex, pixelInstruction.Color);
                        frame.Add(pixelInstruction);
                    }
                }
                
                yield return frame;
                
                // Get the next frames of the used drawers
                foreach (int sectionIndex in providersInNextFrame)
                {
                    timestamps[sectionIndex] += _animationTransformations.GetCorrectedFrameWaitMs(sectionIndex);
                    _framesPerDrawer[sectionIndex].MoveNext();
                }

                // Prepare for the next round
                frameIndex++;
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
