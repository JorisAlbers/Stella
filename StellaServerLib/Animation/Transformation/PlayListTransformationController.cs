using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation.Transformation
{
    /// <summary>
    /// Contains information for a single playlist
    /// </summary>
    public class PlayListTransformationController
    {
        private readonly AnimationTransformationSettings _masterSettings;
        private AnimationTransformationController _masterController;

        public PlayListTransformationSettings Settings { get; }

        public PlayListTransformationController(AnimationTransformationSettings masterSettings)
        {
            _masterSettings = masterSettings;
        }

    }
}
 