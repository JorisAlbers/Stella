using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation.Transformation
{
    public class StoryboardTransformationSettings
    {
        public AnimationTransformationController MasterController { get; }
        public AnimationTransformationController[] AnimationControllers { get; }

        public StoryboardTransformationSettings(AnimationTransformationController masterController, AnimationTransformationController[] animationControllers)
        {
            MasterController = masterController;
            AnimationControllers = animationControllers;
        }
    }
}
