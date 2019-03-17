using System;
using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    public class AnimationRequestProtocol
    {
        private const int REQUEST_BYTES_NEEDED = sizeof(int) + sizeof(int);

        public static byte[] CreateRequest(int startIndex, int count)
        {
            byte[] bytes = new byte[REQUEST_BYTES_NEEDED];
            BitConverter.GetBytes(startIndex).CopyTo(bytes,0);
            BitConverter.GetBytes(count).CopyTo(bytes,sizeof(int));
            return bytes;
        }

        public static void ParseRequest(byte[] bytes, out int startIndex, out int count)
        {
            startIndex = BitConverter.ToInt32(bytes,0);
            count = BitConverter.ToInt32(bytes,sizeof(int));
        }

        // TODO create and parse response
    }
}