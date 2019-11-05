using System.Collections.Generic;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public class AnimatorCreator
    {
        private readonly FrameProviderCreator _frameProviderCreator;
        private readonly int[] _stripLengthPerPi;
        private readonly List<PiMaskItem> _mask;
        private readonly TransformationSettings _masterTransformationSettings;


        public AnimatorCreator(FrameProviderCreator frameProviderCreator, int[] stripLengthPerPi, List<PiMaskItem> mask)
        {
            _frameProviderCreator = frameProviderCreator;
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            _masterTransformationSettings = new TransformationSettings(0,0, new float[3]);
        }

        public IAnimator Create(PlayList playList)
        {
            return new Animator(playList, _frameProviderCreator, _stripLengthPerPi, _mask, _masterTransformationSettings);
        }
    }
}
