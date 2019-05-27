using System.Diagnostics;
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

        [DebuggerStepThrough]
        public PixelInstruction(uint index, Color color)
        {
            Index = index;
            Color = color;
        }

        [DebuggerStepThrough]
        public PixelInstruction(uint index, byte red, byte green, byte blue)
        {
            Index = index;
            Color = Color.FromArgb(red,green,blue);
        }
    }
}