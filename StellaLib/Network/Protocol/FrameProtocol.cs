using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    /// <summary>
    /// Worst case bytes needed:
    ///     300 pixels * 7 + 4 = 2104 (1 strip)
    ///     600 pixels * 7 + 4 = 4208 (2 strips)
    ///     900 pixels * 7 + 4 = 6312 (3 strips)
    /// 
    ///     If we used non-delta:
    ///     300 pixels * 3 + 4 = 904  (1 strip)
    ///     600 pixels * 3 + 4 = 1808 (2 strips)
    ///     900 pixels * 3 + 4 = 2712 (3 strips)
    ///  
    /// </summary>
    public class FrameProtocol
    {
        private const int HEADER_BYTES_NEEDED  = sizeof(int);      // count of pixel changes in the frame
        private const int PIXELINSTRUCTION_BYTES_NEEDED = sizeof(int) + 3;  // index + byte for each color

        public static byte[] SerializeFrame(Frame frame)
        {
            int bytesNeeded = HEADER_BYTES_NEEDED + frame.Count * PIXELINSTRUCTION_BYTES_NEEDED;

            byte[] buffer = new byte[bytesNeeded];
            BitConverter.GetBytes(frame.Count).CopyTo(buffer,0);
            for(int i = 0; i< frame.Count;i++)
            {
                int bufferStartIndex = HEADER_BYTES_NEEDED +  i * PIXELINSTRUCTION_BYTES_NEEDED;

                //Index
                BitConverter.GetBytes(frame[i].Index).CopyTo(buffer,  bufferStartIndex);
                bufferStartIndex += sizeof(int);
                buffer[bufferStartIndex] = frame[i].Color.R;
                bufferStartIndex += 1;
                buffer[bufferStartIndex] = frame[i].Color.G;
                bufferStartIndex += 1;
                buffer[bufferStartIndex] = frame[i].Color.B;
            }
            return buffer;
        }

        public Action<Frame> ReceivedFrame {get;set;}
        private byte[] _headerBuffer;
        private byte[] _frameBuffer;
        private int _bytesReceived;

        // Creates bytes
        public FrameProtocol()
        {
            _headerBuffer = new byte[HEADER_BYTES_NEEDED];
        }

        public void DataReceived(byte[] data)
        {
            int i = 0;
            while(i < data.Length)
            {
                int bytesAvailable = data.Length - i;

                if(_frameBuffer == null)
                {
                    // We're reading into the header buffer
                    int bytesRequested = _headerBuffer.Length - _bytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, _headerBuffer, _bytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    ReadCompleted(bytesTransferred);
                }
                else
                {
                    // We're reading into the pixel instruction buffer
                    int bytesRequested = _frameBuffer.Length - _bytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, _frameBuffer, _bytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    ReadCompleted(bytesTransferred);
                }
            }
        }

    private void ReadCompleted(int count)
    {
        // Get the number of bytes read into the buffer
        _bytesReceived += count;
  
        if(this._frameBuffer == null)
        {
            // We're currently receiving the length buffer
            if (this._bytesReceived != sizeof(int))
            {
                // We haven't gotten all the frame count buffer yet: just wait for more data to arrive
            }
            else
            {
                // We've gotten the length buffer
                int length = BitConverter.ToInt32(this._headerBuffer, 0);
  
                // Sanity check for length < 0
                if (length < 1)
                    throw new System.Net.ProtocolViolationException("Frame size is less than one");
  
                // Create the message type buffer and start reading into it
                this._frameBuffer = new byte[length * PIXELINSTRUCTION_BYTES_NEEDED];
                this._bytesReceived = 0;
            }
        }
        else
        {
            // We're currently receiving the frame buffer
            if (this._bytesReceived != _frameBuffer.Length)
            {
                // We haven't gotten all the frame buffer yet: just wait for more data to arrive
            }
            else
            {
                // We've gotten an entire pixel instruction
                Frame frame = new Frame();

                for(int i = 0; i< _frameBuffer.Length; i += PIXELINSTRUCTION_BYTES_NEEDED)
                {
                    int index = BitConverter.ToInt32(_frameBuffer,i); //Should only take the first 4 bytes
                    byte red   = _frameBuffer[i + 4];
                    byte green = _frameBuffer[i + 5];
                    byte blue  = _frameBuffer[i + 6];
                    frame.Add(new PixelInstruction((uint)index,red,green,blue));
                }
                
                if(this.ReceivedFrame != null)
                {
                    this.ReceivedFrame(frame);
                }

                // Start reading the length buffer again
                _bytesReceived = 0;
                _frameBuffer = null;
            }

        }
        

        
    }
}
}