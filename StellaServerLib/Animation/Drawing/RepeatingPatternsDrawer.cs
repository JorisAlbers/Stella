using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// Repeats one or multiple pattern(s) over the length of the ledstip.
    /// Each frame is a Color[] pattern repeeated over the ledstrip.
    /// </summary>
    public class RepeatingPatternsDrawer : IDrawer
    {
        private readonly int _startIndex;
        private int _lengthStrip;
        private int _frameWaitMS;
        private Color[][] _patterns;

        /// <summary>
        /// Each array of colors in the patterns list will be repeated over the length of the ledstip.
        /// Each frame is the next pattern, repeated.
        /// </summary>
        /// <param name="patterns"></param>
        public RepeatingPatternsDrawer(int startIndex, int lengthStrip, int frameWaitMS, Color[][] patterns)
        {
            _startIndex = startIndex;
            _lengthStrip = lengthStrip;
            _patterns = patterns;
            _frameWaitMS = frameWaitMS;
        }

        /// <inheritdoc />
        public IEnumerator<Frame> GetEnumerator()
        {
            int relativeTimeStamp = 0;
            int frameIndex = 0;
            while (true)
            {
                foreach (Color[] pattern in _patterns)
                {
                    int patternsInStrip = _lengthStrip / pattern.Length;
                    Frame frame = new Frame(frameIndex++, relativeTimeStamp);
                    int leftPixelIndex = 0;
                    for (int j = 0; j < patternsInStrip; j++)
                    {
                        leftPixelIndex = _startIndex + pattern.Length * j;
                        for (int k = 0; k < pattern.Length; k++)
                        {
                            int pixelIndex = (int)leftPixelIndex + k;
                            frame.Add(new PixelInstruction()
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
                        frame.Add(new PixelInstruction()
                        {
                            Index = pixelIndex,
                            Color = pattern[j]
                        });
                    }

                    yield return frame;
                    relativeTimeStamp += _frameWaitMS;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// Repeats one or multiple pattern(s) over the length of the ledstip.
    /// Each frame is a Color[] pattern repeeated over the ledstrip.
    /// </summary>
    public class RepeatingPatternsDrawerWithoutDelta : IDrawerWithoutDelta
    {
        private int _lengthStrip;
        private int _frameWaitMS;
        private Color[][] _patterns;

        /// <summary>
        /// Each array of colors in the patterns list will be repeated over the length of the ledstip.
        /// Each frame is the next pattern, repeated.
        /// </summary>
        /// <param name="patterns"></param>
        public RepeatingPatternsDrawerWithoutDelta(int lengthStrip, int frameWaitMS, Color[][] patterns)
        {
            _lengthStrip = lengthStrip;
            _patterns = patterns;
            _frameWaitMS = frameWaitMS;
        }

        /// <inheritdoc />
        public IEnumerator<FrameWithoutDelta> GetEnumerator()
        {
            int relativeTimeStamp = 0;
            int frameIndex = 0;
            while (true)
            {
                foreach (Color[] pattern in _patterns)
                {
                    int patternsInStrip = _lengthStrip / pattern.Length;
                    FrameWithoutDelta frame = new FrameWithoutDelta(frameIndex++, relativeTimeStamp, _lengthStrip);
                    int leftPixelIndex = 0;
                    for (int j = 0; j < patternsInStrip; j++)
                    {
                        leftPixelIndex = pattern.Length * j;
                        for (int k = 0; k < pattern.Length; k++)
                        {
                            int pixelIndex = leftPixelIndex + k;
                            frame[pixelIndex] = new PixelInstructionWithoutDelta(pattern[k]);
                        }
                    }

                    leftPixelIndex = leftPixelIndex + pattern.Length;
                    // draw remaining pixels of the pattern that does not completely fit on the end of the led strip
                    for (int j = 0; j < _lengthStrip % pattern.Length; j++)
                    {
                        int pixelIndex = leftPixelIndex + j;
                        frame[pixelIndex] = new PixelInstructionWithoutDelta(pattern[j]);
                    }

                    yield return frame;
                    relativeTimeStamp += _frameWaitMS;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}