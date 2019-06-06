using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// Wrapper around multiple drawers. Combines the drawers to a single frame.
    /// This class allows us to draw multiple animations to the same led strip.
    /// </summary>
    public class SectionDrawer : IDrawer
    {
        private readonly IDrawer[] _drawers;
        private readonly int[] _relativeTimestamps;
        private readonly int _firstTimestamp;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="drawers"></param>
        /// <param name="relativeTimestamps">The time to start each frame relative to each other, in milliseconds</param>
        public SectionDrawer(IDrawer[] drawers, int[] relativeTimestamps)
        {
            _drawers = drawers;
            _relativeTimestamps = relativeTimestamps;
            _firstTimestamp = relativeTimestamps.Min();
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            IEnumerator<Frame>[] enumerators = _drawers.Select(x => x.GetEnumerator()).ToArray();
            Frame[] frames = new Frame[_drawers.Length];

            int frameIndex = 0;

            // Initialize frames
            for (int i = 0; i < _drawers.Length; i++)
            {
               enumerators[i].MoveNext();
               frames[i] = enumerators[i].Current;
            }

            while (true)
            {
                // Find the section that will start first
                List<int> drawersInNextFrame = GetNextInLineDrawers(frames);
                if (drawersInNextFrame.Count == 0)
                {
                    throw new Exception("There must always be a next frame");
                }

                // Overwrite the metadata of the frame of the fist drawer.
                Frame frame = frames[drawersInNextFrame[0]];

                // Calculate the timestampRelative
                
                int timestampFirstDrawer = _relativeTimestamps[drawersInNextFrame[0]] + frame.TimeStampRelative;
                int deltaWithOverallTimestamp = timestampFirstDrawer - _firstTimestamp;
                frame.TimeStampRelative = deltaWithOverallTimestamp;
                frame.Index = frameIndex;
                
                // If there are more than one drawers in this frame, add their data to the frame.
                for (int i = 1; i < drawersInNextFrame.Count; i++)
                {
                    frame.AddRange(frames[drawersInNextFrame[i]]);
                }

                yield return frame;



                // Get the next frames of the used drawers
                foreach (int sectionIndex in drawersInNextFrame)
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
        private List<int> GetNextInLineDrawers(Frame[] frames)
        {
            int firstTimestamp = int.MaxValue;
            List<int> sectionIndexes = null;
            for (int i = 0; i < frames.Length; i++)
            {
                int startAt = _relativeTimestamps[i] + frames[i].TimeStampRelative;
                if (startAt < firstTimestamp)
                {
                    firstTimestamp = startAt;
                    sectionIndexes = new List<int>() {i};
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
