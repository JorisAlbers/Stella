using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using StellaLib.Animation;

namespace StellaServer.Animation.Animators
{
    /// <summary>
    /// Reads a bitmap.
    /// Each row in the bitmap is a frame.
    /// </summary>
    public class BitmapAnimator : IAnimator
    {
        private readonly int _stripLength;
        private readonly int _frameWaitMs;
        private readonly Bitmap _bitmap;

        public BitmapAnimator(int stripLength, int frameWaitMS, Bitmap bitmap)
        {
            _stripLength = stripLength;
            _frameWaitMs = frameWaitMS;
            _bitmap = bitmap;
        }

        public List<Frame> Create()
        {
            int width = Math.Min(_bitmap.Width, _stripLength);

            List<Frame> frames = new List<Frame>();
            for (int i = 0; i < _bitmap.Height; i++)
            {
                Frame frame = new Frame(i, i*_frameWaitMs);
                for (int j = 0; j < width; j++)
                {
                    Color color = _bitmap.GetPixel(j, i);
                    frame.Add(new PixelInstruction((uint)j,color));
                }
                frames.Add(frame);
            }

            return frames;
        }
    }
}
