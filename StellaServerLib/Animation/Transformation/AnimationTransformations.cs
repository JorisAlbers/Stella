namespace StellaServerLib.Animation.Transformation
{

    /// <summary>
    /// Used to change animation characteristics during animation.
    /// </summary>
    public class AnimationTransformation
    {
        public AnimationTransformationSettings MasterAnimationTransformationSettings { get;  }

        public AnimationTransformationSettings[] AnimationsTransformationSettings { get;  }


        public AnimationTransformation(AnimationTransformationSettings masterAnimationTransformationSettings, AnimationTransformationSettings[] animationTransformationSettings)
        {
            MasterAnimationTransformationSettings = masterAnimationTransformationSettings;
            AnimationsTransformationSettings = animationTransformationSettings;
        }

        public (byte red, byte green, byte blue) AdjustColor(int index, byte red, byte green, byte blue)
        {
            // TODO slow and not thread safe

            // Convert to floats
            float f_red = red;
            float f_green = green;
            float f_blue = blue;

            // Adjust RGB
            (f_red, f_green, f_blue) = MasterAnimationTransformationSettings.AdjustColor(f_red, f_green, f_blue);
            (f_red, f_green, f_blue) = AnimationsTransformationSettings[index].AdjustColor(f_red, f_green, f_blue);

            // Adjust Brightness
            (f_red, f_green, f_blue) = MasterAnimationTransformationSettings.AdjustBrightness(f_red, f_green, f_blue);
            (f_red, f_green, f_blue) = AnimationsTransformationSettings[index].AdjustBrightness(f_red, f_green, f_blue);

            return ((byte red, byte green, byte blue)) (f_red, f_green, f_blue);

        }

        /// <summary>
        /// Adds the master frame wait ms to the animation frame wait ms
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public int GetCorrectedTimeUnitsPerFrame(int animationIndex)
        {
            return MasterAnimationTransformationSettings.TimeUnitsPerFrame +
                   AnimationsTransformationSettings[animationIndex].TimeUnitsPerFrame;
        }
    }
}
