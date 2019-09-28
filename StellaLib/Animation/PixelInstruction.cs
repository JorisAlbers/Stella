using System.Diagnostics;
using System.Drawing;

namespace StellaLib.Animation
{
    /// <summary>
    /// Struct that contains the index of the pixel to change and its new rgb value.
    /// </summary>
    [DebuggerDisplay("{Index} | {R},{G},{B}")]
    public struct PixelInstructionWithDelta
    {
        /// <summary>
        /// The index of the pixel to change
        /// </summary>
        public int Index;
        
        public byte R;

        public byte G;

        public byte B;

        [DebuggerStepThrough]
        public PixelInstructionWithDelta(int index, byte red, byte green, byte blue)
        {
            Index = index;
            R = red;
            G = green;
            B = blue;
        }

        /// <summary>
        /// Do not use when time efficiency is an issue!
        /// </summary>
        /// <returns></returns>
        public Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }

    [DebuggerDisplay("{R},{G},{B}")]
    public struct PixelInstruction
    {
        public byte R;

        public byte G;

        public byte B;

        [DebuggerStepThrough]
        public PixelInstruction(byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
        }

        /// <summary>
        /// Do not use when time efficiency is an issue!
        /// </summary>
        /// <returns></returns>
        public Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }
    }

    


}