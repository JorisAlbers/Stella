namespace StellaServerLib.Serialization.Animation
{
    /// <summary>
    /// Settings of an animation
    /// </summary>
    public interface IAnimationSettings
    {
        int StartIndex { get; set; }
        int StripLength { get; set; }
        /// <summary> The time at which this animation will start. In milliseconds.  </summary>
        int RelativeStart { get; set; }
    }
}
