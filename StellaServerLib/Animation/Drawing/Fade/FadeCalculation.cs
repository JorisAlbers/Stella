using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StellaServerLib.Animation.Drawing.Fade
{
    public static  class FadeCalculation
    {
        public static  Color[][] CalculateFadedPatterns(Color[] pattern, double frequency, bool startHalfWay)
        {
            var fadeStepsPerPixel = new List<List<Color>>();

            foreach (Color color in pattern)
            {
                var list = new List<Color>();

                // find start
                int i = 0;
                while (true)
                {
                    byte r = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);
                    byte g = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);
                    byte b = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);

                    i++;

                    if (r == 0 && g == 0 && b == 0)
                    {
                        break;
                    }

                    
                }

                bool goneUp = false;
                bool goneHalfWay = !startHalfWay;


                byte r1 = 0, g1 = 0, b1 = 0;

                while (true)
                {
                    byte r = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);
                    byte g = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);
                    byte b = (byte)(Math.Sin(frequency * i) * color.R / 2 + color.R / 2);

                    if (r > 0 || g > 0 || b > 0)
                    {
                        goneUp = true;
                    }

                    if (startHalfWay && r1 > r && g1 > g && b1 > b)
                    {
                        goneHalfWay = true;
                    }

                    if (startHalfWay && goneHalfWay)
                    {
                        list.Add(Color.FromArgb(r, g, b));
                    }

                    r1 = r;
                    g1 = g;
                    b1 = b;


                    if (goneUp && r == 0 && g == 0 && b == 0)
                    {
                        break;
                    }

                    i++;
                }

                fadeStepsPerPixel.Add(list);
            }

            return fadeStepsPerPixel.Select(x => x.ToArray()).ToArray();

        }

    }
}
