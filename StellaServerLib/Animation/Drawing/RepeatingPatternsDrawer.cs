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
        private readonly AnimationTransformation _animationTransformation;
        private Color[][] _patterns;

        /// <summary>
        /// Each array of colors in the patterns list will be repeated over the length of the ledstip.
        /// Each frame is the next pattern, repeated.
        /// </summary>
        public RepeatingPatternsDrawer(int startIndex, int lengthStrip, AnimationTransformation animationTransformation, Color[][] patterns)
        {
            _startIndex = startIndex;
            _lengthStrip = lengthStrip;
            _animationTransformation = animationTransformation;
            _patterns = patterns;
        }

        /// <inheritdoc />
        public IEnumerator<List<PixelInstruction>> GetEnumerator()
        {
            while (true)
            {
                foreach (Color[] pattern in _patterns)
                {
                    List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
                    int patternsInStrip = _lengthStrip / pattern.Length;
                    int leftPixelIndex = 0;

                    for (int j = 0; j < patternsInStrip; j++)
                    {
                        leftPixelIndex = _startIndex + pattern.Length * j;
                        for (int k = 0; k < pattern.Length; k++)
                        {
                            int pixelIndex = (int)leftPixelIndex + k;
                            pixelInstructions.Add(new PixelInstruction()
                            {
                                Index = pixelIndex,
                                Color = pattern[k]
                            });
                        }
                    }

                    leftPixelIndex = leftPixelIndex + pattern.Length;
                    // draw remaining pixels of the pattern that does not completely fit on the end of the led strip
                    for (int j = 0; j < _lengthStrip % pattern.Length; j++)
                    {
                        int pixelIndex = (int)leftPixelIndex + j;
                        pixelInstructions.Add(new PixelInstruction()
                        {
                            Index = pixelIndex,
                            Color = pattern[j]
                        });
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