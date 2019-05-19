using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing
{
    /// <summary>
    /// Reads a bitmap.
    /// Each row in the bitmap is a frame.
    /// </summary>
    public class BitmapDrawer : IDrawer
    {
        private readonly int _frameWaitMs;
        private readonly List<PixelInstruction>[] _frames;

        public BitmapDrawer(int stripLength, int frameWaitMS, Bitmap bitmap)
        {
            // Convert the bitmap to frames
            int width = Math.Min(bitmap.Width, stripLength);
            _frames = new List<PixelInstruction>[bitmap.Height];
            for (int i = 0; i < bitmap.Height; i++)
            {
                List<PixelInstruction> frame = new List<PixelInstruction>();
                for (int j = 0; j < width; j++)
                {
                    Color color = bitmap.GetPixel(j, i);
                    frame.Add(new PixelInstruction((uint)j, color));
                }

                _frames[i] = frame;
            }

            _frameWaitMs = frameWaitMS;
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int frameIndex = 0;
            int relativeTimestamp = 0;
            while (true)
            {
                for (int i = 0; i < _frames.Length; i++)
                {
                    Frame frame = new Frame(frameIndex, relativeTimestamp);
                    frame.AddRange(_frames[i]);
                    yield return frame;

                    frameIndex++;
                    relativeTimestamp += _frameWaitMs;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
