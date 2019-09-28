using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing.Fade
{
    public class FadingPulseDrawer : IDrawer
    {
        private const int MINIMUM_FADEPOINTS_ADDED_EVERY_FRAME = 1;
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly int _fadeSteps;
        private readonly Color[] _fadeColors;
        private Random _random;
        private LinkedList<List<FadePoint>> _fadePointsPerFadeStep;



        public FadingPulseDrawer(int startIndex, int stripLength, Color color, int fadeSteps)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _fadeSteps = fadeSteps;

            Color[][] fadedPatterns = FadeCalculation.CalculateFadedPatterns(new Color[] { color }, _fadeSteps);
            _fadeColors = new Color[_fadeSteps];
            for (int i = 0; i < _fadeSteps; i++)
            {
                _fadeColors[i] = fadedPatterns[i][0];
            }
            _random = new Random();
            _fadePointsPerFadeStep = new LinkedList<List<FadePoint>>();
        }

        private void DrawFadePoints(LinkedList<List<FadePoint>> fadePointsPerFadeStep, List<PixelInstructionWithDelta> pixelInstructions)
        {
            foreach (List<FadePoint> fadePoints in fadePointsPerFadeStep)
            {
                // All fade points at this index have the same fade step count.
                int fadeStep = fadePoints[0].Step;
                Color color = _fadeColors[_fadeColors.Length - 1 - fadeStep];

                foreach (FadePoint fadePoint in fadePoints)
                {
                    for (int j = fadePoint.Point - fadeStep; j < fadePoint.Point + fadeStep + 1; j++)
                    {
                        if (j < 0 || j > _stripLength - 1)
                        {
                            continue;
                        }
                        pixelInstructions.Add(new PixelInstructionWithDelta(_startIndex + j, color.R,color.G,color.B));
                    }

                    fadePoint.Step++;
                }
            }
        }

        private List<FadePoint> CreateNewFadePoints(Random random, int number)
        {
            List<FadePoint> fadePoints = new List<FadePoint>();
            // start random n of new fade points
            for (int i = 0; i < number; i++)
            {
                fadePoints.Add(new FadePoint(random.Next(0,_stripLength)));
            }

            return fadePoints;
        }
       
        public bool MoveNext()
        {
            // Create new FadePoints
            int fadePointsToAdd = _random.Next(MINIMUM_FADEPOINTS_ADDED_EVERY_FRAME, 5);
            if (fadePointsToAdd > 0)
            {
                _fadePointsPerFadeStep.AddLast(CreateNewFadePoints(_random, fadePointsToAdd));
            }

            // draw existing FadePoints
            List<PixelInstructionWithDelta> pixelInstructions = new List<PixelInstructionWithDelta>();
            DrawFadePoints(_fadePointsPerFadeStep, pixelInstructions);

            // remove FadePoints that have elapsed
            if (_fadePointsPerFadeStep.First != null && _fadePointsPerFadeStep.First.Value[0].Step > _fadeSteps - 1)
            {
                _fadePointsPerFadeStep.RemoveFirst();
            }

            Current = pixelInstructions;
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
