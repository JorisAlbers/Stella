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
        private readonly Color[] _pattern;
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly IEnumerator<List<PixelInstructionWithDelta>> _internalEnumerator;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="startIndex">The start index on the led strip</param>
        /// <param name="stripLength">The length of the section to draw</param>
        /// <param name="pattern">The pattern to move</param>
        public MovingPatternDrawer(int startIndex, int stripLength, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _pattern = pattern;
            _internalEnumerator = GetEnumerator();
        }

        public bool MoveNext()
        {
            return _internalEnumerator.MoveNext();
        }

        public void Reset()
        {
            _internalEnumerator.Reset();
        }

        public List<PixelInstructionWithDelta> Current => _internalEnumerator.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
           _internalEnumerator.Dispose();
        }

        private IEnumerator<List<PixelInstructionWithDelta>> GetEnumerator()
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
    }
}
