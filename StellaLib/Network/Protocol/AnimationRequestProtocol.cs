using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network.Protocol.Animation;

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

        public static List<byte[]> CreateResponse(IEnumerable<Frame> frames)
        {
            List<byte[]> packages = new List<byte[]>();
            foreach (Frame frame in frames)
            {
                packages.AddRange(FrameProtocol.SerializeFrame(frame, PacketProtocol.MAX_MESSAGE_SIZE));
            }
            return packages;
        }
    }
}