using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// Repeats a pattern over the ledstrip and moves the start point of the pattern each
    /// frame by +1.
    /// </summary>
    public class SlidingPatternDrawer : IDrawer
    {
        private Color[] _pattern;
        private readonly int _startIndex;
        private int _stripLength;
        private int _frameWaitMS;

        public SlidingPatternDrawer(int startIndex, int stripLength, int frameWaitMS, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int timestampRelative = 0;
            int frameIndex = 0;
            while (true)
            {
                for (int i = 0; i < _pattern.Length; i++)
                {
                    Frame frame = new Frame(frameIndex++, timestampRelative);
                    int patternStart = i;
                    for (int j = 0; j < _stripLength; j++)
                    {
                        frame.Add(new PixelInstruction() { Index = (uint) (_startIndex + j), Color = _pattern[(j + patternStart) % (_pattern.Length)] });
                    }

                    yield return frame;
                    timestampRelative += _frameWaitMS;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}