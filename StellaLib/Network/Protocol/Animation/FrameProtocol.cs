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

        public static byte[][] SerializeFrame(Frame frame, int maxSizePerPackage)
        {
            int bytesNeeded = HEADER_BYTES_NEEDED + frame.Count * PixelInstructionProtocol.BYTES_NEEDED;
            byte[][] packages;

            if(bytesNeeded <= maxSizePerPackage)
            {
                // The frame is small enough to fit in a single packet.
                // No FrameSectionPackage is needed.
                packages = new byte[1][];
                packages[0] = new byte[bytesNeeded];
                BitConverter.GetBytes(frame.Index).CopyTo(packages[0],0);  // Sequence index
                BitConverter.GetBytes(frame.TimeStampRelative).CopyTo(packages[0],4);  // TimeStamp (relative)
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
            int instructionsInFirstSection = (maxSizePerPackage - headerBytesNeeded) / PixelInstructionProtocol.BYTES_NEEDED; 

            // Second, calculate how many FrameSections are needed
            int instructionsThatFitInOtherSections =  (maxSizePerPackage - FrameSectionProtocol.HEADER_BYTES_NEEDED) / PixelInstructionProtocol.BYTES_NEEDED; 
            int frameSectionsNeeded = (int) Math.Ceiling((double)(frame.Count - instructionsInFirstSection) / instructionsThatFitInOtherSections) +1; // +1 for the first section
            
            packages = new byte[frameSectionsNeeded][];

            // Third, create the Frame header and its first section
            packages[0] = new byte[headerBytesNeeded + instructionsInFirstSection * PixelInstructionProtocol.BYTES_NEEDED];

            BitConverter.GetBytes(frame.Index).CopyTo(packages[0],0);           // Sequence index
            BitConverter.GetBytes(frame.TimeStampRelative).CopyTo(packages[0],4);           // TimeStamp (relative)
            BitConverter.GetBytes(frameSectionsNeeded).CopyTo(packages[0],8);    // Number of FrameSets
            BitConverter.GetBytes(true).CopyTo(packages[0],12);                  // Has FrameSections
            CreateFrameSection(packages[0],13,frame,frame.Index,0,0,instructionsInFirstSection);
            
            // Lastely, create the other frame sections. A new byte array (package) for each FrameSection.
            for (int i = 0; i < frameSectionsNeeded-1; i++)
            {
                int instructionStartIndex = instructionsInFirstSection + i * instructionsThatFitInOtherSections;
                int instructionsInThisSection = Math.Min(instructionsThatFitInOtherSections, frame.Count - instructionStartIndex);
                packages[i+1] = new byte[FrameSectionProtocol.HEADER_BYTES_NEEDED + PixelInstructionProtocol.BYTES_NEEDED * instructionsInThisSection];
                CreateFrameSection(packages[i+1],0,frame,frame.Index,i+1,instructionStartIndex,instructionsInThisSection);
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

        /// <summary>
        /// Get the index of a FrameProtocol package. FrameHeader and FrameSection independent.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static int GetFrameIndex(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }

        private Frame _frame;
        private int _numberOfFrameSections;
        private bool[]  _frameSectionsReceived;

        
        /// <summary>
        /// Deserialize a package
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>True if the frame has been deserialized, False if not all packages have been received.</returns>
        public bool TryDeserialize(byte[] bytes, out Frame frame)
        {
            frame = null;
            if(_frame == null)
            {
                // First package.
                // Read Frame header
                int startIndex = 0;
                int indexOfFrame = BitConverter.ToInt32(bytes,startIndex);
                int timeStampRelative = BitConverter.ToInt32(bytes,startIndex+=4);
                int itemCount = BitConverter.ToInt32(bytes,startIndex+=4);
                bool hasFrameSections = BitConverter.ToBoolean(bytes,startIndex+=4);
                startIndex += 1;
                _frame = new Frame(indexOfFrame,timeStampRelative);

                if(hasFrameSections)
                {
                    _numberOfFrameSections = itemCount;
                    _frameSectionsReceived = new bool[itemCount];
                    _frameSectionsReceived[0] = true;
                    // The rest of the package is a FrameSection
                    FrameSectionPackage package = FrameSectionProtocol.Deserialize(bytes,startIndex);
                    foreach (PixelInstruction instruction in package.pixelInstructions)
                    {
                        _frame.Add(instruction);
                    }
                    return false;
                }
                else
                {
                    // The rest of the package contains PixelInstructions
                    for (int i = 0; i < itemCount; i++)
                    {
                        int instructionStartIndex = startIndex + i * PixelInstructionProtocol.BYTES_NEEDED;
                        _frame.Add(PixelInstructionProtocol.Deserialize(bytes,instructionStartIndex));
                    }
                    frame = _frame;
                    return true;
                }
            }
            else
            {
                // Subsequent package. Always starts with a FrameSection.
                FrameSectionPackage package = FrameSectionProtocol.Deserialize(bytes,0);
                if(package.FrameSequenceIndex != _frame.Index)
                {
                    throw new System.Net.ProtocolViolationException(
                        $"The frameIndex of the frameSection does not match with the Frame's index. Expected :{frame.Index}, Received: {package.FrameSequenceIndex}");
                }

                _frameSectionsReceived[package.Index] = true;

                // The order of the pixelinstructions in the frame does not matter as they contain their index.
                foreach (PixelInstruction instruction in package.pixelInstructions)
                {
                    _frame.Add(instruction);
                }

                if(_frameSectionsReceived.All(x=>x))
                {
                    frame = _frame;
                    return true;
                }
                
                return false;
            }
        }
    }
}