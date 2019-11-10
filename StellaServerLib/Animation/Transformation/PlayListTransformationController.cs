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
        private AnimationTransformationController _masterController;

        public PlayListTransformationSettings Settings { get; }

        public PlayListTransformationController(AnimationTransformationController masterController)
        {
            _masterController = masterController;
        }

    }
}
