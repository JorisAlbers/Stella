using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation.PlayLists
{
    /// <summary>
    /// Settings of a playlist
    /// </summary>
    [YamlTag(nameof(PlayList))]
    public class PlayListSettings
    {
        public string Name { get; set; }

        /// <summary>
        /// The storyboards in this playList, by name.
        /// </summary>
        [YamlMember("Storyboards")]
        public PlayListItemSettings[] StoryboardSettings { get; set; }
    }
}
