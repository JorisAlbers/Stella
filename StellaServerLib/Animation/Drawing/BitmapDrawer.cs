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
        private readonly List<PixelInstruction>[] _imageFrames;
        private int _index;

        public static List<PixelInstruction>[] CreateFrames(Bitmap bitmap)
        {
            // Convert the bitmap to frames
            List<PixelInstruction>[] frames = new List<PixelInstruction>[bitmap.Height];
            
            unsafe
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
                int bytesPerPixel = Image.GetPixelFormatSize(bitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* ptrFirstPixel = (byte*)bitmapData.Scan0;

                for (int y = 0; y < heightInPixels; y++)
                {
                    List<PixelInstruction> frame = new List<PixelInstruction>();

                    byte* currentLine = ptrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        frame.Add(new PixelInstruction((byte) oldRed,(byte) oldGreen,(byte) oldBlue));
                    }

                    frames[y] = frame;
                }
                bitmap.UnlockBits(bitmapData);
            }
            return frames;

        }

        /// <param name="wrap">If the bitmap drawer should draw the first line after the last line</param>
        public BitmapDrawer(int startIndex, int stripLength, bool wrap, List<PixelInstruction>[] imageFrames)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _imageFrames = imageFrames;
            _wrap = wrap;
        }
        
        private List<PixelInstructionWithDelta> ConvertToDelta(List<PixelInstruction> originalFrames)
        {
            int width = Math.Min(originalFrames.Count, _stripLength);
            List<PixelInstructionWithDelta> frames = new List<PixelInstructionWithDelta>();
            for (int i = 0; i < width; i++)
            {
                PixelInstruction instruction = originalFrames[i];

                frames.Add(new PixelInstructionWithDelta(_startIndex+i, instruction.R, instruction.G, instruction.B));
            }

            return frames;
        }

      
        public bool MoveNext()
        {
            int index = _index;
            if (_index >= _imageFrames.Length)
            {
                // wrap is active, go from bottom to top.
                index = _imageFrames.Length - (_index - _imageFrames.Length) -1;
            }

            Current = ConvertToDelta(_imageFrames[index]);

            if (_wrap)
            {
                _index = ++_index % (_imageFrames.Length * 2);
            }
            else
            {
                _index = ++_index % _imageFrames.Length;
            }

            return true;
        }

        public void Reset()
        {
            _index = 0;
        }

        public List<PixelInstructionWithDelta> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            ;
        }
    }
}
