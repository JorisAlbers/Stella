using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// Moves a pattern over the led strip from the start of the led strip till the end.
    /// </summary>
    public class MovingPatternDrawer : IDrawer
    {
        private Color[] _pattern;
        private readonly int _startIndex;
        private int _stripLength;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="startIndex">The start index on the led strip</param>
        /// <param name="stripLength">The length of the section to draw</param>
        /// <param name="animationTransformation"></param>
        /// <param name="pattern">The pattern to move</param>
        public MovingPatternDrawer(int startIndex, int stripLength, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _pattern = pattern;
        }


        /// <inheritdoc />
        public IEnumerator<List<PixelInstructionWithDelta>> GetEnumerator()
        {
            while (true)
            {
                // Slide into view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
                    for (int j = 0; j < i + 1; j++)
                    {
                        Color color = _pattern[_pattern.Length - 1 - i + j];
                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + j, color.R, color.G, color.B ));
                    }
                    yield return pixelInstructions;
                }

                // Normal
                for (int i = 0; i < _stripLength - _pattern.Length + 1; i++)
                {
                    List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
                    for (int j = 0; j < _pattern.Length; j++)
                    {
                        Color color = _pattern[j];
                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + i + j, color.R, color.G, color.B));
                    }

                    yield return pixelInstructions;
                }

                // Slide out of view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
                    for (int j = 0; j < _pattern.Length - 1 - i; j++)
                    {
                        Color color = _pattern[j];
                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + (_stripLength - (_pattern.Length - 1 - j - i)), color.R, color.G, color.B));
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
