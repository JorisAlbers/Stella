using System;
using System.Collections.Generic;
using System.Text;
using SharpYaml.Serialization;
using StellaServer.Animation.Drawing;
using StellaServer.Serialization.Animation;

namespace StellaServer.Animation
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
