using System;
using StellaLib.Animation;

namespace StellaLib.Network.Protocol
{
    public class PixelInstructionProtocol
    {
        public const int BYTES_NEEDED = sizeof(int) + 1 + 1 + 1; // Index, Red, Green, Blue
        
        public static byte[] Serialize(PixelInstruction instruction)
        {
            byte[] buffer = new byte[BYTES_NEEDED];
            BitConverter.GetBytes(instruction.Index).CopyTo(buffer,  0);
            buffer[4] = instruction.Color.R;
            buffer[5] = instruction.Color.G;
            buffer[6] = instruction.Color.B;
            return buffer;
        }


    }
}