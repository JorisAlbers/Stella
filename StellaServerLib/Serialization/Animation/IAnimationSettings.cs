namespace StellaServerLib.Serialization.Animation
{
    /// <summary>
    /// Settings of an animation
    /// </summary>
    public interface IAnimationSettings
    {
        int StartIndex { get; set; }
        int StripLength { get; set; }

        /// <summary>
        /// Row = line of led tubes.
        /// 0 based. -1 means not set.
        /// </summary>
        int RowIndex { get; set; }

        /// <summary>
        /// If the animation should take the full matrix of leds.
        /// </summary>
        bool StretchToCanvas { get; set; }

        /// <summary> The time at which this animation will start. In milliseconds.  </summary>
        int RelativeStart { get; set; }
    }
}
