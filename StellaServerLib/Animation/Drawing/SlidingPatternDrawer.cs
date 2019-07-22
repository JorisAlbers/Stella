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

        public IEnumerator<List<PixelInstruction>> GetEnumerator()
        {
            while (true)
            {
                for (int i = 0; i < _pattern.Length; i++)
                {
                    List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
                    int patternStart = i;
                    for (int j = 0; j < _stripLength; j++)
                    {
                        pixelInstructions.Add(new PixelInstruction() { Index = _startIndex + j, Color = _pattern[(j + patternStart) % (_pattern.Length)] });
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