using System;
using System.Collections.Generic;
using System.Text;

namespace StellaServerLib.Animation.Transformation
{
    public class StoryboardTransformationSettings
    {
        public AnimationTransformationSettings MasterSettings { get; }
        public AnimationTransformationSettings[] AnimationSettings{ get; }

        public StoryboardTransformationSettings(AnimationTransformationSettings masterSettings, AnimationTransformationSettings[] animationSettings)
        {
            MasterSettings = masterSettings;
            AnimationSettings = animationSettings;
        }

        public (byte red, byte green, byte blue) AdjustColor(int index, byte red, byte green, byte blue)
        {
            // TODO slow and not thread safe

            // Convert to floats
            float f_red = red;
            float f_green = green;
            float f_blue = blue;

            // Adjust RGB
            (f_red, f_green, f_blue) = MasterSettings.AdjustColor(f_red, f_green, f_blue);
            (f_red, f_green, f_blue) = AnimationSettings[index].AdjustColor(f_red, f_green, f_blue);

            // Adjust Brightness
            (f_red, f_green, f_blue) = MasterSettings.AdjustBrightness(f_red, f_green, f_blue);
            (f_red, f_green, f_blue) = AnimationSettings[index].AdjustBrightness(f_red, f_green, f_blue);

            return ((byte red, byte green, byte blue))(f_red, f_green, f_blue);

        }

        /// <summary>
        /// Adds the master frame wait ms to the animation frame wait ms
        /// </summary>
        /// <param name="animationIndex"></param>
        /// <returns></returns>
        public int GetCorrectedTimeUnitsPerFrame(int animationIndex)
        {
            return MasterSettings.TimeUnitsPerFrame +
                   AnimationSettings[animationIndex].TimeUnitsPerFrame;
        }


    }
}
