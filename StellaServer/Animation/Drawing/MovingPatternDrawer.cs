using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// Moves a pattern over the led strip from the start of the led strip till the end.
    /// </summary>
    public class MovingPatternDrawer : IDrawer
    {
        private Color[] _pattern;
        private readonly int _startIndex;
        private int _stripLength;
        private int _frameWaitMS;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="startIndex">The start index on the led strip</param>
        /// <param name="stripLength">The length of the section to draw</param>
        /// <param name="frameWaitMS">The frame duration</param>
        /// <param name="pattern">The pattern to move</param>
        public MovingPatternDrawer(int startIndex, int stripLength, int frameWaitMS, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
        }


        /// <inheritdoc />
        public IEnumerator<Frame> GetEnumerator()
        {
            int timestampRelative = 0;
            int frameIndex = 0;
            while (true)
            {
                // Slide into view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    Frame frame = new Frame(frameIndex++, timestampRelative);
                    for (int j = 0; j < i + 1; j++)
                    {
                        frame.Add(new PixelInstruction(_startIndex + j, _pattern[_pattern.Length - 1 - i + j]));
                    }
                    yield return frame;
                    timestampRelative += _frameWaitMS;
                }

                // Normal
                for (int i = 0; i < _stripLength - _pattern.Length + 1; i++)
                {
                    Frame frame = new Frame(frameIndex++, timestampRelative);
                    for (int j = 0; j < _pattern.Length; j++)
                    {
                        frame.Add(new PixelInstruction { Index = _startIndex + i + j, Color = _pattern[j] });
                    }

                    yield return frame;
                    timestampRelative += _frameWaitMS;
                }

                // Slide out of view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    Frame frame = new Frame(frameIndex++, timestampRelative);
                    for (int j = 0; j < _pattern.Length - 1 - i; j++)
                    {
                        frame.Add(new PixelInstruction(_startIndex + (_stripLength - (_pattern.Length - 1 - j - i)), _pattern[j]));
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
