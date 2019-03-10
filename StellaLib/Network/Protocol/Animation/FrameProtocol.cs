using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StellaLib.Animation;
using StellaLib.Network.Packages;

namespace StellaLib.Network.Protocol.Animation
{
    /// <summary>
    ///     The frame looks like this:
    ///     HEADER
    ///     int : number of pixel changes
    ///     int : wait time in miliseconds
    ///     PIXELINSTRUCTION 1
    ///         int index of pixel to change
    ///         byte red
    ///         byte green
    ///         byte blue
    ///      PIXELINSTRUCTION 2
    ///         ...
    ///      PIXELINSTRUCTION 3
    ///         ...
    /// 
    ///  
    /// Worst case bytes needed:
    ///     300 pixels * 7 + 8 = 2108 (1 strip)
    ///     600 pixels * 7 + 8 = 4212 (2 strips)
    ///     900 pixels * 7 + 8 = 6316 (3 strips)
    /// 
    ///     If we used non-delta:
    ///     300 pixels * 3 + 8 = 908  (1 strip)
    ///     600 pixels * 3 + 8 = 1812 (2 strips)
    ///     900 pixels * 3 + 8 = 2716 (3 strips)
    ///  
    /// </summary>
    public class FrameProtocol
    {
        // Timestamp (absolute), number of frames OR number of Pixelinstructions, Has FrameSections
        private const int HEADER_BYTES_NEEDED  = sizeof(long) + sizeof(int) + sizeof(bool); 

        public static byte[][] SerializeFrame(Frame frame, int indexOfFrame)
        {
            int bytesNeeded = HEADER_BYTES_NEEDED + frame.Count * PixelInstructionProtocol.BYTES_NEEDED;
            byte[][] packages;

            if(bytesNeeded <= PacketProtocol.MAX_MESSAGE_SIZE)
            {
                // The frame is small enough to fit in a single packet.
                // No FrameSectionPackage is needed.
                packages = new byte[1][];
                packages[0] = new byte[bytesNeeded];
                BitConverter.GetBytes(indexOfFrame).CopyTo(packages[0],0);  // Sequence index
                BitConverter.GetBytes(frame.WaitMS).CopyTo(packages[0],4);  // TimeStamp (relative)
                BitConverter.GetBytes(frame.Count).CopyTo(packages[0],8);   // Number of PixelInstructions
                BitConverter.GetBytes(false).CopyTo(packages[0],12);        // Has FrameSections
                
                for(int i = 0; i< frame.Count;i++)
                {
                    int bufferStartIndex = HEADER_BYTES_NEEDED +  i * PixelInstructionProtocol.BYTES_NEEDED;
                    PixelInstructionProtocol.Serialize(frame[i], packages[0], bufferStartIndex);
                }
                return packages;
            }

            // This frame is too big to send as one packet.
            // Create multiple packages with the help of FrameSectionPackage
            
            // First, calculate the inital package
            int headerBytesNeeded = HEADER_BYTES_NEEDED + FrameSectionProtocol.HEADER_BYTES_NEEDED;
            int instructionsInFirstSection = (PacketProtocol.MAX_MESSAGE_SIZE - headerBytesNeeded) / PixelInstructionProtocol.BYTES_NEEDED; 

            // Second, calculate how many FrameSections are needed
            int instructionsThatFitInOtherSections =  (PacketProtocol.MAX_MESSAGE_SIZE - FrameSectionProtocol.HEADER_BYTES_NEEDED) / PixelInstructionProtocol.BYTES_NEEDED; 
            int frameSectionsNeeded = (int) Math.Ceiling((double)(frame.Count - instructionsInFirstSection) / instructionsThatFitInOtherSections) +1; // +1 for the first section
            
            packages = new byte[frameSectionsNeeded][];

            // Third, create the Frame header and its first section
            packages[0] = new byte[headerBytesNeeded + instructionsInFirstSection * PixelInstructionProtocol.BYTES_NEEDED];
            
            BitConverter.GetBytes(indexOfFrame).CopyTo(packages[0],0);           // Sequence index
            BitConverter.GetBytes(frame.WaitMS).CopyTo(packages[0],4);           // TimeStamp (relative)
            BitConverter.GetBytes(frameSectionsNeeded).CopyTo(packages[0],8);    // Number of FrameSets
            BitConverter.GetBytes(true).CopyTo(packages[0],12);                  // Has FrameSections
            CreateFrameSection(packages[0],13,frame,indexOfFrame,0,0,instructionsInFirstSection);
            
            // Lastely, create the other frame sections. A new byte array (package) for each FrameSection.
            for (int i = 0; i < frameSectionsNeeded-1; i++)
            {
                int instructionStartIndex = instructionsInFirstSection + i * instructionsThatFitInOtherSections;
                int instructionsInThisSection = Math.Min(instructionsThatFitInOtherSections, frame.Count - instructionStartIndex);
                packages[i+1] = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED + PixelInstructionProtocol.BYTES_NEEDED * instructionsInThisSection];
                CreateFrameSection(packages[i+1],0,frame,indexOfFrame,i+1,instructionStartIndex,instructionsInThisSection);
            }
            return packages;
        }

        private static void CreateFrameSection(byte[] buffer,int bufferStartIndex, Frame frame,
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

            FrameSectionProtocol.Serialize(package,buffer,bufferStartIndex);
        }

    }
}