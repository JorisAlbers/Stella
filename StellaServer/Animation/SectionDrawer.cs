using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using StellaLib.Animation;
using StellaServer.Animation.Drawing;

namespace StellaServer.Animation
{
    /// <summary>
    /// Wrapper around multiple drawers. Combines the drawers to a single frame.
    /// </summary>
    public class SectionDrawer : IDrawer
    {
        private readonly IEnumerator<Frame>[] _drawers;
        private readonly Frame[] _frames;
        private readonly int[] _relativeTimestamps;
        private readonly int _firstTimestamp;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="drawers"></param>
        /// <param name="relativeTimestamps">The time to start each frame relative to each other, in milliseconds</param>
        public SectionDrawer(IDrawer[] drawers, int[] relativeTimestamps)
        {
            _drawers = drawers.Select(x=>x.GetEnumerator()).ToArray();
            _relativeTimestamps = relativeTimestamps;
            _firstTimestamp = relativeTimestamps.Min();
            _frames = new Frame[drawers.Length];
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int frameIndex = 0;
            int timestampRelative = 0;

            // Initialize frames
            for (int i = 0; i < _drawers.Length; i++)
            {
               _drawers[i].MoveNext();
               _frames[i] = _drawers[i].Current;
            }

            while (true)
            {
                // Find the section that will start first
                List<int> drawersInNextFrame = GetNextInLineDrawers();
                if (drawersInNextFrame.Count == 0)
                {
                    throw new Exception("There must always be a next frame");
                }

                // Overwrite the metadata of the frame of the fist drawer.
                Frame frame = _frames[drawersInNextFrame[0]];

                // Calculate the timestampRelative
                
                int timestampFirstDrawer = _relativeTimestamps[drawersInNextFrame[0]] + frame.TimeStampRelative;
                int deltaWithOverallTimestamp = timestampFirstDrawer - _firstTimestamp;
                frame.TimeStampRelative = deltaWithOverallTimestamp;
                frame.Index = frameIndex;
                
                // If there are more than one drawers in this frame, add their data to the frame.
                for (int i = 1; i < drawersInNextFrame.Count; i++)
                {
                    frame.AddRange(_frames[drawersInNextFrame[i]]);
                }

                yield return frame;



                // Get the next frames of the used drawers
                foreach (int sectionIndex in drawersInNextFrame)
                {
                    _drawers[sectionIndex].MoveNext();
                    _frames[sectionIndex] = _drawers[sectionIndex].Current;
                }

                // Prepare for the next round
               
                frameIndex++;

            }
        }

        /// <summary>
        /// Returns the indexes of the drawers that have a frame starting before the other drawers.
        /// </summary>
        /// <returns></returns>
        private List<int> GetNextInLineDrawers()
        {
            int firstTimestamp = int.MaxValue;
            List<int> sectionIndexes = null;
            for (int i = 0; i < _frames.Length; i++)
            {
                int startAt = _relativeTimestamps[i] + _frames[i].TimeStampRelative;
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
