using System;
using System.Collections.Generic;
using StellaLib.Animation;
using StellaLib.Network.Packages;

namespace StellaLib.Network.Protocol.Animation
{
    public static class FrameSectionProtocol
    {
        public const int HEADER_BYTES_NEEDED = sizeof(int) + sizeof(int) + sizeof(int); // frameSequenceIndex + Index + number of pixelinstructions;

        public static byte[] Serialize(FrameSectionPackage package, byte[] buffer, int startIndex)
        {
            BitConverter.GetBytes(package.FrameSequenceIndex).CopyTo(buffer, startIndex);
            BitConverter.GetBytes(package.Index).CopyTo(buffer, startIndex + 4);
            BitConverter.GetBytes(package.pixelInstructions.Count).CopyTo(buffer, startIndex + 8);
            int pixelInstructionsStartIndex = startIndex + 12;
            for (int i = 0; i < package.pixelInstructions.Count; i++)
            {
                PixelInstructionProtocol.Serialize(package.pixelInstructions[i], buffer, pixelInstructionsStartIndex + i * PixelInstructionProtocol.BYTES_NEEDED);
            }
            return buffer;
        }

        public static FrameSectionPackage Deserialize(byte[] buffer, int startIndex)
        {
            FrameSectionPackage package = new FrameSectionPackage();
            //Header
            package.FrameSequenceIndex = BitConverter.ToInt32(buffer, startIndex);
            package.Index = BitConverter.ToInt32(buffer, startIndex + 4);
            int numberOfPixelInstructions = BitConverter.ToInt32(buffer, startIndex + 8);
            //Content
            package.pixelInstructions = new List<PixelInstructionWithoutDelta>(numberOfPixelInstructions);
            int pixelInstructionsStartIndex = startIndex + 12;
            for (int i = 0; i < numberOfPixelInstructions; i++)
            {
                package.pixelInstructions.Add(PixelInstructionProtocol.Deserialize(buffer, pixelInstructionsStartIndex + i * PixelInstructionProtocol.BYTES_NEEDED));
            }

            if (package.pixelInstructions.Count != numberOfPixelInstructions)
            {
                throw new System.Net.ProtocolViolationException(
                    $"Failed to deserialize FrameSectionPackage, the number of pixelInstructions incorrect (exp. {numberOfPixelInstructions}, rec. {package.pixelInstructions.Count})");
            }

            return package;
        }
    }
}