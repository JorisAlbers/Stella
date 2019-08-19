using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Animation;
using StellaLib.Network.Packages;

namespace StellaLib.Network.Protocol.Animation
{
    public class FrameWithoutDeltaProtocol
    {
        // Timestamp (absolute), number of frames OR number of Pixelinstructions, Has FrameSections
        private const int HEADER_BYTES_NEEDED = sizeof(long) + sizeof(int) + sizeof(bool);

        public static byte[][] SerializeFrame(FrameWithoutDelta frame, int maxSizePerPackage)
        {
            int bytesNeeded = HEADER_BYTES_NEEDED + frame.Items.Length * PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED;
            byte[][] packages;

            if (bytesNeeded <= maxSizePerPackage) 
            {
                // The frame is small enough to fit in a single packet.
                // No FrameSectionPackage is needed.
                packages = new byte[1][];
                packages[0] = new byte[bytesNeeded];
                BitConverter.GetBytes(frame.Index).CopyTo(packages[0], 0);  // Sequence index
                BitConverter.GetBytes(frame.TimeStampRelative).CopyTo(packages[0], 4);  // TimeStamp (relative)
                BitConverter.GetBytes(frame.Count).CopyTo(packages[0], 8);   // Number of PixelInstructions
                BitConverter.GetBytes(false).CopyTo(packages[0], 12);        // Has FrameSections

                for (int i = 0; i < frame.Count; i++)
                {
                    int bufferStartIndex = HEADER_BYTES_NEEDED + i * PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED;
                    PixelInstructionWithoutDeltaProtocol.Serialize(frame[i], packages[0], bufferStartIndex);
                }
                return packages;
            }

            // This frame is too big to send as one packet.
            // Create multiple packages with the help of FrameSectionPackage

            // First, calculate the inital package
            int headerBytesNeeded = HEADER_BYTES_NEEDED + FrameSectionWithoutDeltaProtocol.HEADER_BYTES_NEEDED;
            int instructionsInFirstSection = (maxSizePerPackage - headerBytesNeeded) / PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED;

            // Second, calculate how many FrameSections are needed
            int instructionsThatFitInOtherSections = (maxSizePerPackage - FrameSectionWithoutDeltaProtocol.HEADER_BYTES_NEEDED) / PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED;
            int frameSectionsNeeded = (int)Math.Ceiling((double)(frame.Count - instructionsInFirstSection) / instructionsThatFitInOtherSections) + 1; // +1 for the first section

            packages = new byte[frameSectionsNeeded][];

            // Third, create the Frame header and its first section
            packages[0] = new byte[headerBytesNeeded + instructionsInFirstSection * PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED];

            BitConverter.GetBytes(frame.Index).CopyTo(packages[0], 0);           // Sequence index
            BitConverter.GetBytes(frame.TimeStampRelative).CopyTo(packages[0], 4);           // TimeStamp (relative)
            BitConverter.GetBytes(frameSectionsNeeded).CopyTo(packages[0], 8);    // Number of FrameSets
            BitConverter.GetBytes(true).CopyTo(packages[0], 12);                  // Has FrameSections
            CreateFrameSection(packages[0], 13, frame, frame.Index, 0, 0, instructionsInFirstSection);

            // Lastely, create the other frame sections. A new byte array (package) for each FrameSection.
            for (int i = 0; i < frameSectionsNeeded - 1; i++)
            {
                int instructionStartIndex = instructionsInFirstSection + i * instructionsThatFitInOtherSections;
                int instructionsInThisSection = Math.Min(instructionsThatFitInOtherSections, frame.Count - instructionStartIndex);
                packages[i + 1] = new byte[FrameSectionWithoutDeltaProtocol.HEADER_BYTES_NEEDED + PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED * instructionsInThisSection];
                CreateFrameSection(packages[i + 1], 0, frame, frame.Index, i + 1, instructionStartIndex, instructionsInThisSection);
            }
            return packages;
        }

        private static void CreateFrameSection(byte[] buffer, int bufferStartIndex, FrameWithoutDelta frame,
         int frameIndex, int sectionIndex, int instructionStartIndex, int numberOfInstructions)
        {
            FrameSectionPackageWithoutDelta package = new FrameSectionPackageWithoutDelta();
            package.FrameSequenceIndex = frameIndex;
            package.Index = sectionIndex;
            package.pixelInstructions = new List<PixelInstructionWithoutDelta>();
            for (int i = instructionStartIndex; i < instructionStartIndex + numberOfInstructions; i++)
            {
                package.pixelInstructions.Add(frame[i]);
            }

            FrameSectionWithoutDeltaProtocol.Serialize(package, buffer, bufferStartIndex);
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

        private FrameSectionPackageWithoutDelta[] _frameSectionPackages;
        private bool[] _frameSectionsReceived;
        private int _frameIndex = -1;
        private int _timeStampRelative = -1;



        /// <summary>
        /// Deserialize a package
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns>True if the frame has been deserialized, False if not all packages have been received.</returns>
        public bool TryDeserialize(byte[] bytes, out FrameWithoutDelta frame)
        {
            frame = null;
            if (_frameSectionPackages == null)
            {
                // First package.
                // Read Frame header
                int startIndex = 0;
                _frameIndex = BitConverter.ToInt32(bytes, startIndex);
                _timeStampRelative = BitConverter.ToInt32(bytes, startIndex += 4);
                int itemCount = BitConverter.ToInt32(bytes, startIndex += 4);
                bool hasFrameSections = BitConverter.ToBoolean(bytes, startIndex += 4);
                startIndex += 1;

                if (hasFrameSections)
                {
                    _frameSectionPackages = new FrameSectionPackageWithoutDelta[itemCount];
                    _frameSectionsReceived = new bool[itemCount];
                    _frameSectionsReceived[0] = true;
                    // The rest of the package is a FrameSection
                    _frameSectionPackages[0] = FrameSectionWithoutDeltaProtocol.Deserialize(bytes, startIndex);
                    return false;
                }
                else
                {
                    frame = new FrameWithoutDelta(_frameIndex, _timeStampRelative, itemCount);
                    // The rest of the package contains PixelInstructions
                    for (int i = 0; i < itemCount; i++)
                    {
                        int instructionStartIndex = startIndex + i * PixelInstructionWithoutDeltaProtocol.BYTES_NEEDED;
                        frame[i] = PixelInstructionWithoutDeltaProtocol.Deserialize(bytes, instructionStartIndex);
                    }
                    return true;
                }
            }
            else
            {
                // Subsequent package. Always starts with a FrameSection.
                FrameSectionPackageWithoutDelta package = FrameSectionWithoutDeltaProtocol.Deserialize(bytes, 0);
                if (package.FrameSequenceIndex != _frameIndex)
                {
                    throw new System.Net.ProtocolViolationException(
                        $"The frameIndex of the frameSection does not match with the Frame's index. Expected :{_frameIndex}, Received: {package.FrameSequenceIndex}");
                }

                _frameSectionPackages[package.Index] = package;
                _frameSectionsReceived[package.Index] = true;

                if (_frameSectionsReceived.All(x => x))
                {
                    // TODO add totalNumberOfPixelInstructions to frame header
                    int totalPixelInstructions = _frameSectionPackages.Sum(x => x.NumberOfPixelInstructions);
                    frame = new FrameWithoutDelta(_frameIndex, _timeStampRelative, totalPixelInstructions);
                    int index = 0;
                    for (int i = 0; i < _frameSectionPackages.Length; i++)
                    {
                        for (int j = 0; j < _frameSectionPackages[i].NumberOfPixelInstructions; j++)
                        {
                            frame[index++] = _frameSectionPackages[i].pixelInstructions[j];
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}