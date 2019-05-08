using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// Repeats one or multiple pattern(s) over the length of the ledstip.
    /// Each frame is a Color[] pattern repeeated over the ledstrip.
    /// </summary>
    public class RepeatingPatternsDrawer : IDrawer
    {
        private int _lengthStrip;
        private int _frameWaitMS;
        private List<Color[]> _patterns;

        /// <summary>
        /// Each array of colors in the patterns list will be repeated over the length of the ledstip.
        /// Each frame is the next pattern, repeated.
        /// </summary>
        /// <param name="patterns"></param>
        public RepeatingPatternsDrawer(int lengthStrip,int frameWaitMS, List<Color[]> patterns)
        {
            _lengthStrip = lengthStrip;
            _patterns = patterns;
            _frameWaitMS = frameWaitMS;
        }

        public List<Frame> Create()
        {
            int relativeTimeStamp = 0;
            List<Frame> frames = new List<Frame>();
            foreach (Color[] pattern in _patterns)
            {
                int patternsInStrip = _lengthStrip / pattern.Length;
                Frame frame = new Frame(frames.Count, relativeTimeStamp);
                int leftPixelIndex = 0;
                for (int j = 0; j < patternsInStrip; j++)
                {
                    leftPixelIndex = pattern.Length * j;
                    for (int k = 0; k < pattern.Length; k++)
                    {
                        int pixelIndex = (int) leftPixelIndex + k;
                        frame.Add (new PixelInstruction ()
                        {
                            Index = (uint) pixelIndex, Color = pattern[k]
                        });
                    }
                }

                leftPixelIndex = leftPixelIndex + pattern.Length;
                // draw remaining pixels of the pattern that does not completely fit on the end of the led strip
                for (int j = 0; j < _lengthStrip % pattern.Length; j++)
                {
                    int pixelIndex = (int) leftPixelIndex + j;
                    frame.Add (new PixelInstruction ()
                    {
                        Index = (uint) pixelIndex, Color = pattern[j]
                    });
                }
                frames.Add (frame);
                relativeTimeStamp += _frameWaitMS;
            }
            return frames;
        }
    }
}