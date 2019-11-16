using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation.PlayLists
{
    public class PlayListItemSettings
    {
        /// <summary>
        /// The name of the storyboard to play. Used as reference to a storyboard.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time the storyboard will be on display, in seconds
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The settings of the storyboard to play. Can be null.
        /// </summary>
        /// <summary> The different animations in this storyboard. </summary>
        [YamlMember("Animations")]
        public IAnimationSettings[] AnimationSettings { get; set; }
    }
}