using SharpYaml.Serialization;
using StellaServerLib.Serialization.Animation;

namespace StellaServerLib.Animation
{
    /// <summary>
    /// Collection of animations.
    /// </summary>
    [YamlTag(nameof(Storyboard))]
    public class Storyboard
    {
        public string Name { get; set; }

        /// <summary> The different animations in this storyboard. </summary>
        [YamlMember("Animations")]
        public IAnimationSettings[] AnimationSettings { get; set; }

    }
}
