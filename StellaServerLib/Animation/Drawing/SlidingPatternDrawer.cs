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
        private Color[] _pattern;
        private readonly int _startIndex;
        private int _stripLength;

        public SlidingPatternDrawer(int startIndex, int stripLength, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _pattern = pattern;
        }

        public IEnumerator<List<PixelInstructionWithDelta>> GetEnumerator()
        {
            while (true)
            {
                for (int i = 0; i < _pattern.Length; i++)
                {
                    List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
                    int patternStart = i;
                    for (int j = 0; j < _stripLength; j++)
                    {
                        Color color = _pattern[(j + patternStart) % (_pattern.Length)];
                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + j, color.R,color.G,color.B));
                    }

                    yield return pixelInstructions;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}