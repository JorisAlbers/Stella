using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// Repeats a pattern over the ledstrip and moves the start point of the pattern each
    /// frame by +1.
    /// </summary>
    public class SlidingPatternDrawer : IDrawer
    {
        private Color[] _pattern;
        private int _stripLength;
        private int _frameWaitMS;

        public SlidingPatternDrawer(int stripLength, int frameWaitMS, Color[] pattern)
        {
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
        }

        public List<Frame> Create()
        {
            int timestampRelative = 0;
            List<Frame> frames = new List<Frame>();
            for (int i = 0; i < _pattern.Length; i++)
            {
                Frame frame = new Frame(frames.Count,timestampRelative);
                int startIndex = i;
                for (int j = 0; j < _stripLength; j++) 
                {
                    frame.Add(new PixelInstruction(){ Index = (uint)j, Color = _pattern[(j + startIndex) % (_pattern.Length)] });
                }
                frames.Add(frame);
                timestampRelative += _frameWaitMS;
            }
            return frames;
        }
    }
}