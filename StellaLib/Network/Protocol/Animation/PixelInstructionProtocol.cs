using System;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol.Animation
{
    public static class PixelInstructionProtocol
    {
        public const int BYTES_NEEDED = 3; // Red, Green, Blue

        public static byte[] Serialize(PixelInstruction instruction, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = instruction.R;
            buffer[startIndex + 1] = instruction.G;
            buffer[startIndex + 2] = instruction.B;
            return buffer;
        }

        public static PixelInstruction Deserialize(byte[] bytes, int startIndex)
        {
            PixelInstruction pixelInstruction = new PixelInstruction();
            pixelInstruction.R = bytes[startIndex];
            pixelInstruction.G = bytes[startIndex + 1];
            pixelInstruction.B = bytes[startIndex + 2];
            return pixelInstruction;
        }
    }
}