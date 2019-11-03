using System;
using System.Collections.Generic;
using System.Text;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public class AnimatorCreator
    {
        private readonly FrameProviderCreator _frameProviderCreator;
        private readonly TransformationSettings _masterTransformationSettings;


        public AnimatorCreator(FrameProviderCreator frameProviderCreator)
        {
            _frameProviderCreator = frameProviderCreator;
            _masterTransformationSettings = new TransformationSettings(0,0, new float[3]);
        }

        public IAnimator Create(Storyboard storyboard, int[] stripLengthPerPi, List<PiMaskItem> mask)
        {
            return new Animator(_frameProviderCreator.Create(storyboard, _masterTransformationSettings, out TransformationController transformationController),
                stripLengthPerPi, mask, transformationController);
        }
    }
}
