using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StellaServerLib.Animation.Transformation
{
    // Contains settings for a single storyboard.
    public class StoryboardTransformationController
    {
        private AnimationTransformationController _masterController;
        private AnimationTransformationController[] _animationControllers;

        public StoryboardTransformationSettings Settings { get; }

        public StoryboardTransformationController(AnimationTransformationController masterController, AnimationTransformationSettings[] animationSettings)
        {
            _masterController = masterController;
            _animationControllers = animationSettings.Select(x => new AnimationTransformationController(x)).ToArray();

            Settings = new StoryboardTransformationSettings(_masterController, _animationControllers);
        }
    }
}
