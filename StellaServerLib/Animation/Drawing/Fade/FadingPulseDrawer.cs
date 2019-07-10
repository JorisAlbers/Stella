﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServerLib.Animation.Drawing.Fade
{
    public class FadingPulseDrawer : IDrawer
    {
        private readonly int _startIndex;
        private readonly int _stripLength;
        private readonly AnimationTransformation _animationTransformation;
        private readonly int _fadeSteps;
        private readonly Color[] _fadeColors; 

        private const int MINIMUM_FADEPOINTS_ADDED_EVERY_FRAME = 1;
        

        public FadingPulseDrawer(int startIndex, int stripLength, AnimationTransformation animationTransformation, Color color, int fadeSteps)
        {
            _startIndex = startIndex;
            _stripLength = stripLength;
            _animationTransformation = animationTransformation;
            _fadeSteps = fadeSteps;

            Color[][] fadedPatterns = FadeCalculation.CalculateFadedPatterns(new Color[] { color }, _fadeSteps);
            _fadeColors = new Color[_fadeSteps];
            for (int i = 0; i < _fadeSteps; i++)
            {
                _fadeColors[i] = fadedPatterns[i][0];
            }
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
                int fadePointsToAdd = random.Next(MINIMUM_FADEPOINTS_ADDED_EVERY_FRAME, 5);
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

        private void DrawFadePoints(LinkedList<List<FadePoint>> fadePointsPerFadeStep, Frame frame)
        {
            foreach (List<FadePoint> fadePoints in fadePointsPerFadeStep)
            {
                // All fade points at this index have the same fade step count.
                int fadeStep = fadePoints[0].Step;
                Color color = _animationTransformation.AdjustColor(_fadeColors[_fadeColors.Length - 1 - fadeStep]);

                foreach (FadePoint fadePoint in fadePoints)
                {
                    for (int j = fadePoint.Point - fadeStep; j < fadePoint.Point + fadeStep + 1; j++)
                    {
                        if (j < 0 || j > _stripLength - 1)
                        {
                            continue;
                        }
                        frame.Add(new PixelInstruction(_startIndex + j, color));
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


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
