using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation.PlayLists
{
    public class PlayListItemSettings
    {
        /// <summary>
        /// The name of the storyboard to play.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The time the storyboard will be on display, in seconds
        /// </summary>
        public int Duration { get; set; }
    }
}