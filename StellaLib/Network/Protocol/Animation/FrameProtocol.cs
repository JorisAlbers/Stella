using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Animation;
using StellaLib.Network.Packages;

namespace StellaLib.Network.Protocol.Animation
{
    public class FrameProtocol
    {
        // Frame index, total sections, number of pixel instructions in this package
        private const int FRAME_HEADER_BYTES_NEEDED = sizeof(int) + sizeof(int) + sizeof(int);



        public static byte[][] SerializeFrame(FrameWithoutDelta frame, int maxSizePerPackage)
        {
            int bytesNeeded = FRAME_HEADER_BYTES_NEEDED + frame.Items.Length * PixelInstructionProtocol.BYTES_NEEDED;
            byte[][] packages;

            //int packagesNeeded = 

            if (bytesNeeded <= maxSizePerPackage) 
            {
                // The frame is small enough to fit in a single packet.
                // No FrameSectionPackage is needed.
                packages = new byte[1][];
                packages[0] = new byte[bytesNeeded];
                BitConverter.GetBytes(frame.Index).CopyTo(packages[0], 0);  // Frame index
                BitConverter.GetBytes(1).CopyTo(packages[0], 4);   // Number of sections (one)
                BitConverter.GetBytes(frame.Count).CopyTo(packages[0], 8);   // Number of PixelInstructions

                for (int i = 0; i < frame.Count; i++)
                {
                    int bufferStartIndex = FRAME_HEADER_BYTES_NEEDED + i * PixelInstructionProtocol.BYTES_NEEDED;
                    PixelInstructionProtocol.Serialize(frame[i], packages[0], bufferStartIndex);
                }
                return packages;
            }

            // This frame is too big to send as one packet.
            // Create multiple packages with the help of FrameSectionPackage

            int instructionsInFrameHeader = (maxSizePerPackage - FRAME_HEADER_BYTES_NEEDED) / PixelInstructionProtocol.BYTES_NEEDED;
            int instructionsInFrameSection = (maxSizePerPackage - FrameSectionProtocol.HEADER_BYTES_NEEDED) / PixelInstructionProtocol.BYTES_NEEDED;
            int frameSectionsNeeded = ((frame.Count - instructionsInFrameHeader) + instructionsInFrameSection -1) / instructionsInFrameSection;

            packages = new byte[frameSectionsNeeded + 1 ][]; // +1 as the frame header is a package too.

            // Create the Frame header

            packages[0] = new byte[FRAME_HEADER_BYTES_NEEDED + instructionsInFrameHeader * PixelInstructionProtocol.BYTES_NEEDED];

            BitConverter.GetBytes(frame.Index).CopyTo(packages[0], 0);           // Sequence index
            BitConverter.GetBytes(frameSectionsNeeded +1).CopyTo(packages[0], 4);    // Number of frame sections (sections + frame header)
            BitConverter.GetBytes(instructionsInFrameHeader).CopyTo(packages[0], 8);    // Number of pixel instructions
            // Add pixel instructions
            int pixelInstructionsStartIndex = 12;
            for (int i = 0; i <instructionsInFrameHeader; i++)
            {
                PixelInstructionProtocol.Serialize(frame[i], packages[0], pixelInstructionsStartIndex + i * PixelInstructionProtocol.BYTES_NEEDED);
            }

            // Last, create the other frame sections. A new byte array (package) for each FrameSection.
            for (int i = 0; i < frameSectionsNeeded; i++)
            {
                int instructionStartIndex = instructionsInFrameHeader + i * instructionsInFrameSection;
                int instructionsInThisSection = Math.Min(instructionsInFrameSection, frame.Count - instructionStartIndex);
                packages[i + 1] = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED + PixelInstructionProtocol.BYTES_NEEDED * instructionsInThisSection];
                CreateFrameSection(packages[i + 1], 0, frame, frame.Index, i + 1, instructionStartIndex, instructionsInThisSection);
            }
            return packages;
        }

        private static void CreateFrameSection(byte[] buffer, int bufferStartIndex, FrameWithoutDelta frame,
         int frameIndex, int sectionIndex, int instructionStartIndex, int numberOfInstructions)
        {
            FrameSectionPackage package = new FrameSectionPackage();
            package.FrameSequenceIndex = frameIndex;
            package.Index = sectionIndex;
            package.pixelInstructions = new List<PixelInstruction>();
            for (int i = instructionStartIndex; i < instructionStartIndex + numberOfInstructions; i++)
            {
                package.pixelInstructions.Add(frame[i]);
            }

            FrameSectionProtocol.Serialize(package, buffer, bufferStartIndex);
        }
        
    }
}