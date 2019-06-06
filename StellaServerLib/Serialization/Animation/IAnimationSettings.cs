namespace StellaServerLib.Serialization.Animation
{
    /// <summary>
    /// Settings of an animation
    /// </summary>
    public interface IAnimationSettings
    {
        int StartIndex { get; set; }
        int StripLength { get; set; }
        int FrameWaitMs { get; set; }
    }
}
