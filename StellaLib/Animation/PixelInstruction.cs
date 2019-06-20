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
        public int Index;

        /// <summary>
        /// The color to set the pixel to
        /// </summary>
        public Color Color;

        [DebuggerStepThrough]
        public PixelInstruction(int index, Color color)
        {
            Index = index;
            Color = color;
        }

        [DebuggerStepThrough]
        public PixelInstruction(int index, byte red, byte green, byte blue)
        {
            Index = index;
            Color = Color.FromArgb(red,green,blue);
        }
    }

    public struct PixelInstructionWithoutDelta
    {
        /// <summary>
        /// The color to set the pixel to
        /// </summary>
        public Color Color;

        [DebuggerStepThrough]
        public PixelInstructionWithoutDelta(Color color)
        {
            Color = color;
        }

        [DebuggerStepThrough]
        public PixelInstructionWithoutDelta(byte red, byte green, byte blue)
        {
            Color = Color.FromArgb(red, green, blue);
        }
    }


}