﻿using System.Collections;
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
        private readonly AnimationTransformation _animationTransformation;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="startIndex">The start index on the led strip</param>
        /// <param name="stripLength">The length of the section to draw</param>
        /// <param name="animationTransformation"></param>
        /// <param name="pattern">The pattern to move</param>
        public MovingPatternDrawer(int startIndex, int stripLength, AnimationTransformation animationTransformation, Color[] pattern)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _animationTransformation = animationTransformation;
            _pattern = pattern;
        }


        /// <inheritdoc />
        public IEnumerator<List<PixelInstruction>> GetEnumerator()
        {
            while (true)
            {
                // Slide into view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
                    for (int j = 0; j < i + 1; j++)
                    {
                        pixelInstructions.Add(new PixelInstruction(_startIndex + j, _pattern[_pattern.Length - 1 - i + j]));
                    }
                    yield return pixelInstructions;
                }

                // Normal
                for (int i = 0; i < _stripLength - _pattern.Length + 1; i++)
                {
                    List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
                    for (int j = 0; j < _pattern.Length; j++)
                    {
                        pixelInstructions.Add(new PixelInstruction { Index = _startIndex + i + j, Color = _pattern[j] });
                    }

                    yield return pixelInstructions;
                }

                // Slide out of view
                for (int i = 0; i < _pattern.Length - 1; i++)
                {
                    List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
                    for (int j = 0; j < _pattern.Length - 1 - i; j++)
                    {
                        pixelInstructions.Add(new PixelInstruction(_startIndex + (_stripLength - (_pattern.Length - 1 - j - i)), _pattern[j]));
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
