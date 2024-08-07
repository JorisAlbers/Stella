﻿using System;
using System.Drawing;
using System.Text;

namespace StellaServerAPI.Protocol
{
    public class BitmapProtocol
    {
        private int _pixelsReceived;
        private int _rowsReceived;
        private int _nameLength;
        private Bitmap _bitmap;
        private string _name;



        public BitmapProtocol()
        {
            _pixelsReceived = 0;
            _rowsReceived = 0;
        }

        /// <summary>
        /// True if all data has been received
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool TryDeserialize(byte[] buffer, out Bitmap bitmap, out string name)
        {
            int bufferStartIndex = 0;
            if (_bitmap == null)
            {
                // This is the first package
                _nameLength = BitConverter.ToInt32(buffer, 0);
                int numberOfPixels = BitConverter.ToInt32(buffer, 4);
                int numberOfRows = BitConverter.ToInt32(buffer, 8);
                bufferStartIndex = 12;

                _bitmap = new Bitmap(numberOfPixels, numberOfRows);
            }

            if (_name == null)
            {
                // We are receiving the name.
                // Assumes the name fits in a single package
                _name = Encoding.ASCII.GetString(buffer, bufferStartIndex, _nameLength);
                bufferStartIndex += _nameLength;
            }

            // From this point on, the buffer contains rgb values for each pixel. 
            // So [R, G, B, R, G, B, ...]
            for (int i = bufferStartIndex; i < buffer.Length; i+= 3)
            {
                byte red   = buffer[i];
                byte green = buffer[i+1];
                byte blue  = buffer[i+2];

                _bitmap.SetPixel(_pixelsReceived, _rowsReceived, Color.FromArgb(red,green,blue));

                if (++_pixelsReceived == _bitmap.Width)
                {
                    _pixelsReceived = 0;
                    if (++_rowsReceived == _bitmap.Height)
                    {
                        bitmap = _bitmap;
                        name = _name;
                        return true;
                    }
                }
            }

            bitmap = null;
            name = null;
            return false;
        }

    }
}
