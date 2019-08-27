namespace StellaServerLib.Animation.Drawing
{
    /// <summary>
    /// The different draw methods available.
    /// </summary>
    public enum DrawMethod
    {
        /// <summary> Not set. </summary>
        Unknown,

        /// <summary> Repeats a pattern over the LedStrip and moves the start point of the pattern each frame by +1. </summary>
        SlidingPattern,

        /// <summary> Repeats one or multiple pattern(s) over the length of the LedStrip.
        /// Each frame is a Color[] pattern repeated over the LedStrip. </summary>
        RepeatingPattern,

        /// <summary> Moves a pattern over the led strip from the start of the led strip till the end. </summary>
        MovingPattern,

        /// <summary> Duplicates a pattern a random number of times. The patterns create are randomly placed and fade over time</summary>
        RandomFade,

        /// <summary> Creates a fading pulse in both directions, starting from a given position </summary>
        FadingPulse,

        /// <summary> Scans an image to create an image where x,y = x = pixelIndex, y = frameIndex. </summary>
        Bitmap,



        
    }
}
