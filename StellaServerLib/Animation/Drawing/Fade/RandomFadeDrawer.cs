using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing.Fade
{
    public class RandomFadeDrawer : IDrawer
    {
        private readonly Color[][] _fadePatterns;
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly Random _random;
        private LinkedList<List<FadePoint>> _fadePointsPerFadeStep;


        public RandomFadeDrawer(int startIndex, int stripLength, Color[] pattern, int fadeSteps)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;

            _fadePatterns = FadeCalculation.CalculateFadedPatterns(pattern, 0.2, true);
            _random = new Random();
            _fadePointsPerFadeStep = new LinkedList<List<FadePoint>>();
        }

        private List<PixelInstructionWithDelta> CreateFrame(int fadePointsToAdd,Random random, LinkedList<List<FadePoint>> fadePointsPerFadeStep)
        {
            if (fadePointsToAdd > 0)
            {
                fadePointsPerFadeStep.AddLast(CreateNewFadePoints(random, fadePointsToAdd));
            }

            // draw existing FadePoints
            List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
            DrawFadePoints(fadePointsPerFadeStep, pixelInstructions);

            // remove FadePoints that have elapsed
            if (fadePointsPerFadeStep.First != null && fadePointsPerFadeStep.First.Value[0].Step > _fadePatterns[0].Length -1)
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
        
        private void DrawFadePoints(LinkedList<List<FadePoint>> fadePointsPerFadeStep, List<PixelInstructionWithDelta> pixelInstructions )
        {
            foreach (List<FadePoint> fadePoints in fadePointsPerFadeStep)
            {
                // All fade points at this index have the same fade step count.
                int fadeStep = fadePoints[0].Step;

                foreach (FadePoint fadePoint in fadePoints)
                {
                    for (int i = 0; i < _fadePatterns[fadeStep].Length; i++)
                    {
                        int pixelIndex = i + fadePoint.Point;
                        if (pixelIndex < 0 || pixelIndex > _stripLength - 1)
                        {
                            continue;
                        }

                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + pixelIndex, _fadePatterns[fadeStep][i].R, _fadePatterns[fadeStep][i].G, _fadePatterns[fadeStep][i].B));
                    }
                    fadePoint.Step++;
                }
            }
        }
        

        public bool MoveNext()
        {
            // Create new FadePoints
            Current = CreateFrame(_random.Next(1, 10), _random, _fadePointsPerFadeStep);
            return true;
        }

        public void Reset()
        {
            _fadePointsPerFadeStep = new LinkedList<List<FadePoint>>();
        }

        public List<PixelInstructionWithDelta> Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            ;
        }
    }
}
