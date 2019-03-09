using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    /// <summary>
    ///     The frame looks like this:
    ///     HEADER
    ///     int : number of pixel changes
    ///     int : wait time in miliseconds
    ///     PIXELINSTRUCTION 1
    ///         int index of pixel to change
    ///         byte red
    ///         byte green
    ///         byte blue
    ///      PIXELINSTRUCTION 2
    ///         ...
    ///      PIXELINSTRUCTION 3
    ///         ...
    /// 
    ///  
    /// Worst case bytes needed:
    ///     300 pixels * 7 + 8 = 2108 (1 strip)
    ///     600 pixels * 7 + 8 = 4212 (2 strips)
    ///     900 pixels * 7 + 8 = 6316 (3 strips)
    /// 
    ///     If we used non-delta:
    ///     300 pixels * 3 + 8 = 908  (1 strip)
    ///     600 pixels * 3 + 8 = 1812 (2 strips)
    ///     900 pixels * 3 + 8 = 2716 (3 strips)
    ///  
    /// </summary>
    public class FrameProtocol
    {
        private const int HEADER_BYTES_NEEDED  = sizeof(int) + sizeof(int); // number of pixel changes + waitms

        public static byte[] SerializeFrame(Frame frame)
        {
            int bytesNeeded = HEADER_BYTES_NEEDED + frame.Count * PixelInstructionProtocol.BYTES_NEEDED;

            byte[] buffer = new byte[bytesNeeded];
            BitConverter.GetBytes(frame.Count).CopyTo(buffer,0);
            BitConverter.GetBytes(frame.WaitMS).CopyTo(buffer,sizeof(int));
            for(int i = 0; i< frame.Count;i++)
            {
                int bufferStartIndex = HEADER_BYTES_NEEDED +  i * PixelInstructionProtocol.BYTES_NEEDED;
                PixelInstructionProtocol.Serialize(frame[i], buffer, bufferStartIndex);
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
            if (this._bytesReceived != HEADER_BYTES_NEEDED)
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
                this._frameBuffer = new byte[length * PixelInstructionProtocol.BYTES_NEEDED];
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
                // We've gotten an entire frame
                int numberOfPixelInstructions = BitConverter.ToInt32(_headerBuffer,0);
                int waitMS = BitConverter.ToInt32(_headerBuffer,sizeof(int));

                Frame frame = new Frame(waitMS);

                for(int i = 0; i< _frameBuffer.Length; i += PixelInstructionProtocol.BYTES_NEEDED)
                {
                    frame.Add(PixelInstructionProtocol.Deserialize(_frameBuffer,i));
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