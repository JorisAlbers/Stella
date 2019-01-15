using System.Drawing;

namespace StellaLib.Animation
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

        /// <summary>
        /// The color to set the pixel to
        /// </summary>
        public Color Color;
    }
}