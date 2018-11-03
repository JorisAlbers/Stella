namespace StellaLib
{
    /// <summary>
    /// Struct that contains the index of the pixel to change and its new rgb value.
    /// </summary>
    public struct PixelInstruction
    {
        /// <summary>
        /// The index of the pixel to change
        /// </summary>
        public uint Index;

        public byte Red;

        public byte Green;

        public byte Blue;
    }
}