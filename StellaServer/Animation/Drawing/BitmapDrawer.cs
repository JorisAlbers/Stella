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
        private readonly int _stripLength;
        private readonly int _frameWaitMs;
        private readonly Bitmap _bitmap;

        public BitmapDrawer(int stripLength, int frameWaitMS, Bitmap bitmap)
        {
            _stripLength = stripLength;
            _frameWaitMs = frameWaitMS;
            _bitmap = bitmap;
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int width = Math.Min(_bitmap.Width, _stripLength);
            while (true)
            {
                for (int i = 0; i < _bitmap.Height; i++)
                {
                    Frame frame = new Frame(i, i * _frameWaitMs);
                    for (int j = 0; j < width; j++)
                    {
                        Color color = _bitmap.GetPixel(j, i);
                        frame.Add(new PixelInstruction((uint)j, color));
                    }

                    yield return frame;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
