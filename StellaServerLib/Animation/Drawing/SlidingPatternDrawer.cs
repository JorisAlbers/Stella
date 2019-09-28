using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// Repeats a pattern over the ledstrip and moves the start point of the pattern each
    /// frame by +1.
    /// </summary>
    public class SlidingPatternDrawer : IDrawer
    {
        private readonly Color[] _pattern;
        private readonly int _startIndex;
        private readonly int _stripLength;
        private int _index;

        public SlidingPatternDrawer(int startIndex, int stripLength, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _pattern = pattern;
        }

        public bool MoveNext()
        {
            List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
            for (int j = 0; j < _stripLength; j++)
            {
                Color color = _pattern[(j + _index) % (_pattern.Length)];
                pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + j, color.R, color.G, color.B));
            }

            _index = ++_index % _pattern.Length;
            Current = pixelInstructions;
            return true;
        }

        public void Reset()
        {
            _index = 0;
        }

        public List<PixelInstructionWithDelta> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            ;
        }
    }
}