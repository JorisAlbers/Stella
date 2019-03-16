using System;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol.Animation
{
    public static class FrameSetProtocol
    {
        private const int BYTES_NEEDED = sizeof(long) + sizeof(int); // TimeStamp relative + number of frames

        /// <summary>
        /// Serializes the frameSet header.
        /// </summary>
        /// <param name="frameSet"></param>
        public static byte[] Serialize(FrameSet frameSet)
        {
            byte[] bytes = new byte[BYTES_NEEDED];
            BitConverter.GetBytes(frameSet.TimeStamp.Ticks).CopyTo(bytes,0);  // TimeStamp index
            BitConverter.GetBytes(frameSet.Count).CopyTo(bytes,sizeof(long));  // Number of Frames
            return bytes;
        }

        public static FrameSet Deserialize(byte[] bytes)
        {
            DateTime timeStamp = new DateTime(BitConverter.ToInt64(bytes,0));
            //int numberOfFrames = BitConverter.ToInt32(bytes,sizeof(long));
            return new FrameSet(timeStamp);
        }

    }
}