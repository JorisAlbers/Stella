using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public interface IFrameProviderCreator
    {
        IFrameProvider Create(Storyboard storyboard, AnimationTransformationSettings masterAnimationTransformationSettings,
            out TransformationController transformationController);
    }
}