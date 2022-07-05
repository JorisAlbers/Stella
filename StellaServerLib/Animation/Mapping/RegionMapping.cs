namespace StellaServerLib.Animation.Mapping
{
    /// <summary>
    /// A mapping allows us to transform coordinates to a certain pi and a certain section
    /// </summary>

    public class RegionMapping
    {
        /// <summary> The index of the pi </summary>
        public int PiIndex { get; }

        /// <summary> The length on the combined sequence </summary>
        public int Length { get; }

        /// <summary> The start index on the led strip of the pi </summary>
        public int StartIndexOnPi { get; }

        /// <summary>
        /// If the direction of the mapping is inverted
        /// </summary>
        public bool InverseDirection { get; }
        
        public RegionMapping(int piIndex, int length, int startIndexOnPi,  bool inverseDirection)
        {
            PiIndex = piIndex;
            Length = length;
            StartIndexOnPi = startIndexOnPi;
            InverseDirection = inverseDirection;
        }
    }
}