using System;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol.Animation
{
    public static class FrameSetMetadataProtocol
    {
        private const int BYTES_NEEDED = sizeof(long); // TimeStamp relative

        /// <summary>
        /// Serializes the frameSet header.
        /// </summary>
        /// <param name="frameSet"></param>
        public static byte[] Serialize(FrameSetMetadata metdata)
        {
            byte[] bytes = new byte[BYTES_NEEDED];
            BitConverter.GetBytes(metdata.TimeStamp.Ticks).CopyTo(bytes,0);  // TimeStamp index
            return bytes;
        }

        public static FrameSetMetadata Deserialize(byte[] bytes)
        {
            DateTime timeStamp = new DateTime(BitConverter.ToInt64(bytes,0));
            return new FrameSetMetadata(timeStamp);
        }

    }
}