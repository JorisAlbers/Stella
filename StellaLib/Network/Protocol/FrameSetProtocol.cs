using System;
using System.Collections;
using System.Collections.Generic;
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
    public class FrameSetProtocol
    {
        public static byte[] SerializeFrame(Frame frame)
        {
            int headerBytesNeeded   = sizeof(int);      // count of pixel changes in the frame
            int bytesPerFrameNeeded = sizeof(int) + 3;  // index + byte for each color
            int bytesNeeded = headerBytesNeeded + frame.Count * bytesPerFrameNeeded;

            byte[] buffer = new byte[bytesNeeded];
            BitConverter.GetBytes(frame.Count).CopyTo(buffer,0);
            for(int i = 0; i< frame.Count;i++)
            {
                int bufferStartIndex = headerBytesNeeded +  i * bytesPerFrameNeeded;

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

        private FrameSet _frameSet;
        private List<byte> _buffer;

        // Creates bytes
        public FrameSetProtocol()
        {
            _frameSet = new FrameSet();
            _buffer = new List<byte>(PacketProtocol.BUFFER_SIZE);
        }

        
    }
}