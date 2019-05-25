namespace StellaServer.Animation.Mapping
{
    /// <summary>
    /// A mapping allows us to transform coordinates to a certain pi and a certain section
    /// </summary>

    public class PiMapping
    {
        /// <summary> The index of the pi </summary>
        public int PiIndex { get; }

        /// <summary> The start index on the combined led strip, combining all pis </summary>
        ///  TODO removable if we assume the piMappings are ordered and start from 0.
        public int StartIndex { get; }

        /// <summary> The length on the combined sequence </summary>
        public int Length { get; }

        /// <summary> The start index on the led strip of the pi </summary>
        public int StartIndexOnPi { get; }

        /// <summary>
        /// The sections inside this mapping. 
        /// The section start positions. Each item means a switch in direction from that position.
        ///
        /// If startIndex = 100
        /// And the length = 15
        /// 
        /// 5,    means [0,1,2,3,4, 14,13,12,11,10,9,8,7,6,5] means [
        /// 5, 10 means [0,1,2,3,4, 9,8,7,6,5 10,11,12,13,14]
        ///
        /// When FirstSectionIsInverted:
        ///
        /// 5,    means [4,3,2,1,0  5,6,7,8,9,10,11,12,13,14]
        /// 5, 10 means [4,3,2,1,0  5,6,7,8,9, 14,13,12,11,10]
        /// 
        /// </summary>
        public int[] SectionStarts { get; }

        public bool FirstSectionIsInverted { get; }
        
        public PiMapping(int piIndex, int startIndex,int length, int startIndexOnPi, int[] sectionStarts, bool firstSectionIsInverted)
        {
            PiIndex = piIndex;
            StartIndex = startIndex;
            Length = length;
            StartIndexOnPi = startIndexOnPi;
            SectionStarts = sectionStarts;
            FirstSectionIsInverted = firstSectionIsInverted;
        }
    }
}