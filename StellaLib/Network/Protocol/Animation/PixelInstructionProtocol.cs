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
            buffer[startIndex + 4] = instruction.Color.R;
            buffer[startIndex + 5] = instruction.Color.G;
            buffer[startIndex + 6] = instruction.Color.B;
            return buffer;
        }

        public static PixelInstruction Deserialize(byte[] bytes, int startIndex)
        {
            PixelInstruction pixelInstruction = new PixelInstruction();
            pixelInstruction.Index = BitConverter.ToInt32(bytes,startIndex);
            pixelInstruction.Color = Color.FromArgb(bytes[startIndex+4], bytes[startIndex+5], bytes[startIndex+6]);
            return pixelInstruction;
        }
    }

    public class PixelInstructionWithoutDeltaProtocol
    {
        public const int BYTES_NEEDED = 3; // Red, Green, Blue

        public static byte[] Serialize(PixelInstructionWithoutDelta instruction, byte[] buffer, int startIndex)
        {
            buffer[startIndex] = instruction.Color.R;
            buffer[startIndex + 1] = instruction.Color.G;
            buffer[startIndex + 2] = instruction.Color.B;
            return buffer;
        }

        public static PixelInstructionWithoutDelta Deserialize(byte[] bytes, int startIndex)
        {
            PixelInstructionWithoutDelta pixelInstruction = new PixelInstructionWithoutDelta();
            pixelInstruction.Color = Color.FromArgb(bytes[startIndex], bytes[startIndex + 1], bytes[startIndex + 2]);
            return pixelInstruction;
        }
    }
}