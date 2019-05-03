using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StellaServer.Animation.Animators.Fade
{
    public static  class FadeCalculation
    {
        public static  Color[][] CalculateFadedPatterns(Color[] pattern, int fadeSteps)
        {
            // Calculate increments
            float[][] increments = new float[pattern.Length][];
            for (int i = 0; i < pattern.Length; i++)
            {
                float[] incrementsForPixel = new float[3];
                incrementsForPixel[0] = pattern[i].R / fadeSteps;
                incrementsForPixel[1] = pattern[i].G / fadeSteps;
                incrementsForPixel[2] = pattern[i].B / fadeSteps;
                increments[i] = incrementsForPixel;
            }

            // Get the possible patterns given the number of fade steps.
            // From black to the color

            // FS1 -> Color
            // FS2 -> Color
            // FS3 -> Color

            Color[][] fadePatterns = new Color[fadeSteps+1][];
            for (int i = 0; i < fadeSteps; i++)
            {
                fadePatterns[i] = new Color[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                {
                    float[] incrementForPixel = increments[j];

                    byte red = (byte)(incrementForPixel[0] * i);
                    byte green = (byte)(incrementForPixel[1] * i);
                    byte blue = (byte)(incrementForPixel[2] * i);
                    fadePatterns[i][j] = Color.FromArgb(red, green, blue);
                }
            }
            // Add the pattern as last step
            fadePatterns[fadeSteps] = pattern;

            return fadePatterns;
        }

    }
}
