using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using StellaLib.Animation;

namespace StellaServer.Animation.Animators
{
    /// <summary>
    /// Moves a pattern over the led strip from the start of the led strip till the end.
    /// </summary>
    public class MovingPatternAnimator : IAnimator
    {
        private Color[] _pattern;
        private int _stripLength;
        private int _frameWaitMS;

        public MovingPatternAnimator(int stripLength, int frameWaitMS, Color[] pattern)
        {
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
        }

        public List<Frame> Create()
        {
            int timestampRelative = 0;
            List<Frame> frames = new List<Frame>();

            // Slide into view
            for (int i = 0; i < _pattern.Length-1; i++)
            {
                Frame frame = new Frame(frames.Count, timestampRelative);
                for (uint j = 0; j < i+1; j++)
                {
                    frame.Add(new PixelInstruction(j ,_pattern[_pattern.Length -1 - i + j]));
                }
                frames.Add(frame);
                timestampRelative += _frameWaitMS;
            }

            // Normal
            for (uint i = 0; i < _stripLength - _pattern.Length + 1; i++)
            {
                Frame frame = new Frame(frames.Count, timestampRelative);
                for (uint j = 0; j < _pattern.Length; j++)
                {
                    frame.Add(new PixelInstruction{ Index = i + j, Color = _pattern[j] });
                }

                frames.Add(frame);
                timestampRelative += _frameWaitMS;
            }

            // Slide out of view
            for (int i = 0; i < _pattern.Length - 1; i++)
            {
                Frame frame = new Frame(frames.Count, timestampRelative);
                for (uint j = 0; j < _pattern.Length - 1 - i; j++)
                {
                    frame.Add(new PixelInstruction((uint) (_stripLength - (_pattern.Length - 1  -  j - i)), _pattern[j]));
                }
                frames.Add(frame);
                timestampRelative += _frameWaitMS;
            }

            return frames;
        }
    }
}
