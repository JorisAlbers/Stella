﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// Reads a bitmap.
    /// Each row in the bitmap is a frame.
    ///
    /// The bitmap drawer loops top->bottom , bottom->top
    /// </summary>
    public class BitmapDrawer : IDrawer
    {
        private readonly int _frameWaitMs;
        private readonly bool _wrap;
        private readonly List<PixelInstruction>[] _frames;


        /// <param name="startIndex"></param>
        /// <param name="stripLength"></param>
        /// <param name="frameWaitMS"></param>
        /// <param name="wrap">If the bitmap drawer should draw the first line after the last line</param>
        /// <param name="bitmap"></param>
        public BitmapDrawer(int startIndex, int stripLength, int frameWaitMS, bool wrap, Bitmap bitmap)
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
                    frame.Add(new PixelInstruction(startIndex + j, color));
                }

                _frames[i] = frame;
            }

            _frameWaitMs = frameWaitMS;
            _wrap = wrap;
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            int frameIndex = 0;
            int relativeTimestamp = 0;
            while (true)
            {
                // Top to bottom
                for (int i = 0; i < _frames.Length; i++)
                {
                    Frame frame = new Frame(frameIndex, relativeTimestamp);
                    frame.AddRange(_frames[i]);
                    yield return frame;

                    frameIndex++;
                    relativeTimestamp += _frameWaitMs;
                }

                if (_wrap)
                {
                    // Bottom to top
                    for (int i = _frames.Length - 1; i >= 0; i--)
                    {
                        Frame frame = new Frame(frameIndex, relativeTimestamp);
                        frame.AddRange(_frames[i]);
                        yield return frame;

                        frameIndex++;
                        relativeTimestamp += _frameWaitMs;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
