using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// Repeats one or multiple pattern(s) over the length of the led strip.
    /// Each frame is a Color[] pattern repeated over the led strip.
    /// </summary>
    public class RepeatingPatternsDrawer : IDrawer
    {
        private readonly int _startIndex;
        private int _lengthStrip;
        private Color[][] _patterns;
        private int _index;

        /// <summary>
        /// Each array of colors in the patterns list will be repeated over the length of the ledstip.
        /// Each frame is the next pattern, repeated.
        /// </summary>
        public RepeatingPatternsDrawer(int startIndex, int lengthStrip, Color[][] patterns)
        {
            _startIndex = startIndex;
            _lengthStrip = lengthStrip;
            _patterns = patterns;
        }

        
        public bool MoveNext()
        {
            Color[] currentPattern = _patterns[_index];

            List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
            int patternsInStrip = _lengthStrip / currentPattern.Length;
            int leftPixelIndex = 0;

            for (int j = 0; j < patternsInStrip; j++)
            {
                leftPixelIndex = _startIndex + currentPattern.Length * j;
                for (int k = 0; k < currentPattern.Length; k++)
                {
                    int pixelIndex = leftPixelIndex + k;
                    pixelInstructions.Add(new PixelInstructionWithDelta(pixelIndex, currentPattern[k].R, currentPattern[k].G,
                        currentPattern[k].B));
                }
            }

            leftPixelIndex = leftPixelIndex + currentPattern.Length;
            // draw remaining pixels of the pattern that does not completely fit on the end of the led strip
            for (int j = 0; j < _lengthStrip % currentPattern.Length; j++)
            {
                int pixelIndex = leftPixelIndex + j;
                pixelInstructions.Add(
                    new PixelInstructionWithDelta(pixelIndex, currentPattern[j].R, currentPattern[j].G, currentPattern[j].B));
            }

            Current = pixelInstructions;
            _index = ++_index % _patterns.Length;
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