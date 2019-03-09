using System;
using System.Drawing;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
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
            pixelInstruction.Index = (uint) BitConverter.ToInt32(bytes,startIndex);
            pixelInstruction.Color = Color.FromArgb(bytes[startIndex+4], bytes[startIndex+5], bytes[startIndex+6]);
            return pixelInstruction;
        }
    }
}