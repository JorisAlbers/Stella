using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly bool _wrap;
        private readonly List<PixelInstructionWithoutDelta>[] _imageFrames;

        public static List<PixelInstructionWithoutDelta>[] CreateFrames(Bitmap bitmap)
        {
            // Convert the bitmap to frames
            List<PixelInstructionWithoutDelta>[] frames = new List<PixelInstructionWithoutDelta>[bitmap.Height];
            
            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    List<PixelInstructionWithoutDelta> frame = new List<PixelInstructionWithoutDelta>();

                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        frame.Add(new PixelInstructionWithoutDelta(Color.FromArgb(oldRed,oldGreen,oldBlue)));
                    }

                    frames[y] = frame;
                }
                bitmap.UnlockBits(bitmapData);
            }
            return frames;

        }

        /// <param name="wrap">If the bitmap drawer should draw the first line after the last line</param>
        public BitmapDrawer(int startIndex, int stripLength, bool wrap, List<PixelInstructionWithoutDelta>[] imageFrames)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _imageFrames = imageFrames;
            _wrap = wrap;
        }

        public IEnumerator<List<PixelInstruction>> GetEnumerator()
        {
            while (true)
            {
                // Top to bottom
                for (int i = 0; i < _imageFrames.Length; i++)
                {
                    yield return ConvertToDelta(_imageFrames[i]);
                }

                if (_wrap)
                {
                    // Bottom to top
                    for (int i = _imageFrames.Length - 1; i >= 0; i--)
                    {
                        yield return ConvertToDelta(_imageFrames[i]);
                    }
                }
            }
        }

        private List<PixelInstruction> ConvertToDelta(List<PixelInstructionWithoutDelta> originalFrames)
        {
            int width = Math.Min(originalFrames.Count, _stripLength);
            List<PixelInstruction> frames = new List<PixelInstruction>();
            for (int i = 0; i < width; i++)
            {
                frames.Add(new PixelInstruction(_startIndex+i, originalFrames[i].Color));
            }

            return frames;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
