using System;
using System.Drawing;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol.Animation
{
    public class PixelInstructionProtocol
    {
        public const int BYTES_NEEDED = sizeof(int) + 1 + 1 + 1; // Index, Red, Green, Blue
        
        public static byte[] Serialize(PixelInstruction instruction, byte[] buffer, int startIndex)
        {
            BitConverter.GetBytes(instruction.Index).CopyTo(buffer, startIndex);
            buffer[startIndex + 4] = instruction.R;
            buffer[startIndex + 5] = instruction.G;
            buffer[startIndex + 6] = instruction.B;
            return buffer;
        }

        public static PixelInstruction Deserialize(byte[] bytes, int startIndex)
        {
            PixelInstruction pixelInstruction = new PixelInstruction();
            pixelInstruction.Index = BitConverter.ToInt32(bytes,startIndex);
            pixelInstruction.R = bytes[startIndex + 4];
            pixelInstruction.G = bytes[startIndex + 5];
            pixelInstruction.B = bytes[startIndex + 6];
            return pixelInstruction;
        }
    }

    public class PixelInstructionWithoutDeltaProtocol
    {
        public const int BYTES_NEEDED = 3; // Red, Green, Blue

        public static byte[] Serialize(PixelInstructionWithoutDelta instruction, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = instruction.R;
            buffer[startIndex + 1] = instruction.G;
            buffer[startIndex + 2] = instruction.B;
            return buffer;
        }

        public static PixelInstructionWithoutDelta Deserialize(byte[] bytes, int startIndex)
        {
            PixelInstructionWithoutDelta pixelInstruction = new PixelInstructionWithoutDelta();
            pixelInstruction.R = bytes[startIndex];
            pixelInstruction.G = bytes[startIndex + 1];
            pixelInstruction.B = bytes[startIndex + 2];
            return pixelInstruction;
        }
    }
}