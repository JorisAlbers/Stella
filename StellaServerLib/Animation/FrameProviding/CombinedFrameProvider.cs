using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using StellaLib.Animation;

namespace StellaServerLib.Animation.FrameProviding
{
    /// <summary>
    /// Wrapper around multiple FrameProviders. Combines the FrameProviders to a single frame.
    /// This class allows us to draw multiple animations to the same led strip.
    /// </summary>
    public class CombinedFrameProvider : IFrameProvider
    {
        private readonly IFrameProvider[] _frameProviders;
        private readonly int[] _relativeStartingTimestamps;
        private readonly int _firstTimestamp;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="frameProviders"></param>
        /// <param name="relativeStartingTimestamps">The time to start each frame relative to each other, in milliseconds</param>
        public CombinedFrameProvider(IFrameProvider[] frameProviders, int[] relativeStartingTimestamps)
        {
            _frameProviders = frameProviders;
            _relativeStartingTimestamps = relativeStartingTimestamps;
            _firstTimestamp = relativeStartingTimestamps.Min();
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            IEnumerator<Frame>[] enumerators = _frameProviders.Select(x => x.GetEnumerator()).ToArray();
            Frame[] frames = new Frame[_frameProviders.Length];

            int frameIndex = 0;

            // Initialize frames
            for (int i = 0; i < _frameProviders.Length; i++)
            {
                enumerators[i].MoveNext();
                frames[i] = enumerators[i].Current;
            }

            while (true)
            {
                // Find the section that will start first
                List<int> providersInNextFrame = GetNextInLineProviders(frames);
                if (providersInNextFrame.Count == 0)
                {
                    throw new Exception("There must always be a next frame");
                }

                // Overwrite the metadata of the frame of the fist provider.
                Frame frame = frames[providersInNextFrame[0]];

                // Calculate the timestampRelative

                int timestampFirstDrawer = _relativeStartingTimestamps[providersInNextFrame[0]] + frame.TimeStampRelative;
                int deltaWithOverallTimestamp = timestampFirstDrawer - _firstTimestamp;
                frame.TimeStampRelative = deltaWithOverallTimestamp;
                frame.Index = frameIndex;

                // If there are more than one provider  in this frame, add their data to the frame.
                for (int i = 1; i < providersInNextFrame.Count; i++)
                {
                    frame.AddRange(frames[providersInNextFrame[i]]);
                }

                yield return frame;



                // Get the next frames of the used drawers
                foreach (int sectionIndex in providersInNextFrame)
                {
                    enumerators[sectionIndex].MoveNext();
                    frames[sectionIndex] = enumerators[sectionIndex].Current;
                }

                // Prepare for the next round
                frameIndex++;
            }
        }

        /// <summary>
        /// Returns the indexes of the drawers that have a frame starting before the other drawers.
        /// </summary>
        /// <returns></returns>
        private List<int> GetNextInLineProviders(Frame[] frames)
        {
            int firstTimestamp = int.MaxValue;
            List<int> sectionIndexes = null;
            for (int i = 0; i < frames.Length; i++)
            {
                int startAt = _relativeStartingTimestamps[i] + frames[i].TimeStampRelative;
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
