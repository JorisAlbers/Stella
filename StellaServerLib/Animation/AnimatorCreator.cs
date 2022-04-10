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
        private Animator _previousAnimator;


        public AnimatorCreator(FrameProviderCreator frameProviderCreator, int[] stripLengthPerPi, List<PiMaskItem> mask)
        {
            _frameProviderCreator = frameProviderCreator;
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            
        }

        public IAnimator Create(PlayList playList)
        {
            AnimationTransformationSettings masterTransformationSettings;
            if (_previousAnimator == null)
            {
                masterTransformationSettings = new AnimationTransformationSettings(10, 0, new float[3]{1,1,1});
            }
            else
            {
                masterTransformationSettings =
                    _previousAnimator.StoryboardTransformationController.Settings.MasterSettings;
            }

            _previousAnimator = new Animator(playList, _frameProviderCreator, _stripLengthPerPi, _mask, masterTransformationSettings);
            return _previousAnimator;
        }
    }
}
