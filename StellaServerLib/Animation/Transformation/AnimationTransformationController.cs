using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation.Transformation
{
    // Contains an AnimationTransformationSettings.
    public class AnimationTransformationController
    {
        /// <summary>
        /// The settings of a single animation (Made by an IDrawer)
        /// </summary>
        public AnimationTransformationSettings Settings { get; }

        public AnimationTransformationController(AnimationTransformationSettings settings)
        {
            Settings = settings;
        }
        
    }
}
