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
        private readonly AnimationTransformation _animationTransformation;
        private readonly int _fadeSteps;


        public RandomFadeDrawer(int startIndex, int stripLength, AnimationTransformation animationTransformation, Color[] pattern, int fadeSteps)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _animationTransformation = animationTransformation;
            _pattern = pattern;
            _fadeSteps = fadeSteps;

            _fadePatterns = FadeCalculation.CalculateFadedPatterns(_pattern, _fadeSteps);
        }

        public IEnumerator<Frame> GetEnumerator()
        {
            Random random = new Random();
            int frameIndex = 0;
            int timestampRelative = 0;
            LinkedList<List<FadePoint>> fadePointsPerFadeStep = new LinkedList<List<FadePoint>>();
            while (true)
            {
                // Create new FadePoints
                int fadePointsToAdd = random.Next(0, 10);
                if (fadePointsToAdd > 0)
                {
                    fadePointsPerFadeStep.AddLast(CreateNewFadePoints(random, fadePointsToAdd));
                }

                // draw existing FadePoints
                Frame frame = new Frame(frameIndex++, timestampRelative += _animationTransformation.FrameWaitMs);
                DrawFadePoints(fadePointsPerFadeStep, frame);

                // remove FadePoints that have elapsed
                if (fadePointsPerFadeStep.First != null && fadePointsPerFadeStep.First.Value[0].Step > _fadeSteps - 1)
                {
                    fadePointsPerFadeStep.RemoveFirst();
                }

                yield return frame;
            }
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
        
        private void DrawFadePoints(LinkedList<List<FadePoint>> fadePointsPerFadeStep, Frame frame)
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

                        frame.Add(new PixelInstruction(_startIndex + pixelIndex, pattern[i]));
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
