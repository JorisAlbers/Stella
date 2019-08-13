using System.Drawing;

namespace StellaServerLib.Animation.Transformation
{

    /// <summary>
    /// Used to change animation characteristics during animation.
    /// </summary>
    public class AnimationTransformation
    {
        public TransformationSettings MasterTransformationSettings { get;  }

        public TransformationSettings[] AnimationsTransformationSettings { get;  }


        public AnimationTransformation(TransformationSettings masterTransformationSettings, TransformationSettings[] animationTransformationSettings)
        {
            MasterTransformationSettings = masterTransformationSettings;
            AnimationsTransformationSettings = animationTransformationSettings;
        }

        public Color AdjustColor(int index, Color color)
        {
            // TODO slow and not thread safe
            // Adjust RGB
            color = MasterTransformationSettings.AdjustColor(color);
            color = AnimationsTransformationSettings[index].AdjustColor(color);

            // Adjust Brightness
            color = MasterTransformationSettings.AdjustBrightness(color);
            return AnimationsTransformationSettings[index].AdjustBrightness(color);
        }

        /// <summary>
        /// Adds the master frame wait ms to the animation frame wait ms
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public int GetCorrectedFrameWaitMs(int animationIndex)
        {
            return MasterTransformationSettings.FrameWaitMs +
                   AnimationsTransformationSettings[animationIndex].FrameWaitMs;
        }
    }
}
