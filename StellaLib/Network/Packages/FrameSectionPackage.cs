using System.Collections.Generic;
using StellaLib.Animation;

namespace StellaLib.Network.Packages
{
    public class FrameSectionPackage
    {
        // Header
        public int FrameSequenceIndex {get;set;}
        public int Index {get;set;}
        public int NumberOfPixelInstructions {get{return pixelInstructions.Count;}}
        //Content
        public List<PixelInstruction> pixelInstructions{get;set;}
    }

    public class FrameSectionPackageWithoutDelta
    {
        // Header
        public int FrameSequenceIndex { get; set; }
        public int Index { get; set; }
        public int NumberOfPixelInstructions { get { return pixelInstructions.Count; } }
        //Content
        public List<PixelInstructionWithoutDelta> pixelInstructions { get; set; }
    }
}