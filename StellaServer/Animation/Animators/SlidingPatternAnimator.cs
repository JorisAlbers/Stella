using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Animators
{
    /// <summary>
    /// Repeats a pattern over the ledstrip and moves the start point of the pattern each
    /// frame by +1.
    /// </summary>
    public class SlidingPatternAnimator : IAnimator
    {
        private Color[] _pattern;
        private int _stripLength;
        private int _frameWaitMS;

        public SlidingPatternAnimator(int stripLength, int frameWaitMS, Color[] pattern)
        {
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
        }

        public FrameSet Create()
        {
            FrameSet frameSet = new FrameSet();
            for (int i = 0; i < _pattern.Length; i++)
            {
                Frame frame = new Frame(_frameWaitMS);
                int startIndex = i;
                for (int j = 0; j < _stripLength; j++) 
                {
                    frame.Add(new PixelInstruction(){ Index = (uint)j, Color = _pattern[(j + startIndex) % (_pattern.Length)] });
                }
                frameSet.Add(frame);
            }
            return frameSet;
        }
    }
}