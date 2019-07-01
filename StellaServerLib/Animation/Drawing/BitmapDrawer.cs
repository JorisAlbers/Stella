using System;
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
        private readonly AnimationTransformation _animationTransformation;
        private readonly bool _wrap;
        private readonly List<PixelInstruction>[] _frames;


        /// <param name="wrap">If the bitmap drawer should draw the first line after the last line</param>
        public BitmapDrawer(int startIndex, int stripLength, AnimationTransformation animationTransformation, bool wrap, Bitmap bitmap)
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

            _animationTransformation = animationTransformation;
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
                    yield return CreateFrame(_frames[i], ref frameIndex, ref relativeTimestamp);
                }

                if (_wrap)
                {
                    // Bottom to top
                    for (int i = _frames.Length - 1; i >= 0; i--)
                    {
                        yield return CreateFrame(_frames[i], ref frameIndex, ref relativeTimestamp);
                    }
                }
            }
        }

        private Frame CreateFrame(List<PixelInstruction> instructions, ref int frameIndex, ref int relativeTimestamp)
        {
            Frame frame = new Frame(frameIndex, relativeTimestamp);
            for (int i = 0; i < instructions.Count; i++)
            {
                PixelInstruction instruction = instructions[i];
                instruction.Color = _animationTransformation.AdjustColor(instruction.Color);
                frame.Add(instruction);
            }

            frameIndex++;
            relativeTimestamp += _animationTransformation.FrameWaitMs;
            return frame;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
