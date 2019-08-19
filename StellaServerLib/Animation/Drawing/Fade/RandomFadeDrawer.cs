using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing.Fade
{
    public class RandomFadeDrawer : IDrawer
    {
        private readonly Color[] _pattern;
        private readonly Color[][] _fadePatterns;
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly int _fadeSteps;


        public RandomFadeDrawer(int startIndex, int stripLength, Color[] pattern, int fadeSteps)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _pattern = pattern;
            _fadeSteps = fadeSteps;

            _fadePatterns = FadeCalculation.CalculateFadedPatterns(_pattern, _fadeSteps);
        }

        public IEnumerator<List<PixelInstruction>> GetEnumerator()
        {
            Random random = new Random();
            LinkedList<List<FadePoint>> fadePointsPerFadeStep = new LinkedList<List<FadePoint>>();

            // The first frame should always start with some points
            yield return CreateFrame(random.Next(1, 10), random ,fadePointsPerFadeStep);

            while (true)
            {
                // Create new FadePoints
                yield return CreateFrame(random.Next(0, 10), random, fadePointsPerFadeStep);
            }
        }

        private List<PixelInstruction> CreateFrame(int fadePointsToAdd,Random random, LinkedList<List<FadePoint>> fadePointsPerFadeStep)
        {
            if (fadePointsToAdd > 0)
            {
                fadePointsPerFadeStep.AddLast(CreateNewFadePoints(random, fadePointsToAdd));
            }

            // draw existing FadePoints
            List<PixelInstruction> pixelInstructions = new List<PixelInstruction>();
            DrawFadePoints(fadePointsPerFadeStep, pixelInstructions);

            // remove FadePoints that have elapsed
            if (fadePointsPerFadeStep.First != null && fadePointsPerFadeStep.First.Value[0].Step > _fadeSteps - 1)
            {
                fadePointsPerFadeStep.RemoveFirst();
            }

            return pixelInstructions;
        }


        private List<FadePoint> CreateNewFadePoints(Random random, int number)
        {
            List<FadePoint> fadePoints = new List<FadePoint>();
            // start random n of new fade points
            for (int i = 0; i < number; i++)
            {
                fadePoints.Add(new FadePoint(random.Next(0, _stripLength)));
            }

            return fadePoints;
        }
        
        private void DrawFadePoints(LinkedList<List<FadePoint>> fadePointsPerFadeStep, List<PixelInstruction> pixelInstructions )
        {
            foreach (List<FadePoint> fadePoints in fadePointsPerFadeStep)
            {
                // All fade points at this index have the same fade step count.
                int fadeStep = fadePoints[0].Step;
                Color[] pattern = _fadePatterns[_fadeSteps - 1 - fadeStep];

                foreach (FadePoint fadePoint in fadePoints)
                {
                    for (int i = 0; i < pattern.Length; i++)
                    {
                        int pixelIndex = i + fadePoint.Point;
                        if (pixelIndex < 0 || pixelIndex > _stripLength - 1)
                        {
                            continue;
                        }

                        pixelInstructions.Add(new PixelInstruction(_startIndex + pixelIndex, pattern[i].R, pattern[i].G, pattern[i].B));
                    }
                    fadePoint.Step++;
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
