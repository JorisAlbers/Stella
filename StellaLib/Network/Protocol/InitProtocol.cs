using System;
using System.Collections.Generic;
using System.Text;

namespace StellaLib.Network.Protocol
{
    public class InitProtocol
    {
        private const int BYTES_NEEDED = sizeof(int) * 3;
        private const int version = 1;

        public static byte[] Serialize(int pixels, byte brightness)
        {
            int startIndex= 0;
            byte[] buffer = new byte[BYTES_NEEDED];

            BitConverter.GetBytes(version).CopyTo(buffer, startIndex);
            startIndex += sizeof(int);


            BitConverter.GetBytes(pixels).CopyTo(buffer, startIndex);
            startIndex += sizeof(int);

            BitConverter.GetBytes(brightness).CopyTo(buffer, startIndex);
            startIndex += sizeof(int);

            return buffer;
        }
    }
}
