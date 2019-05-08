using System;
using System.Collections.Generic;
using System.Drawing;
using StellaLib.Animation;

namespace StellaServer.Animation.Drawing.Fade
{
    public class RandomFadeDrawer : IDrawer
    {
        private readonly Color[] _pattern;
        private readonly int _stripLength;
        private readonly int _frameWaitMS;
        private readonly int _numberOfFadeCycles;
        private readonly int _fadeSteps;
        private readonly int _maxPatternsVisible;

        public RandomFadeDrawer(int stripLength, int frameWaitMS, Color[] pattern, int fadeSteps, int maxPatternsVisible, int numberOfFadeCycles)
        {
            _stripLength = stripLength;
            _frameWaitMS = frameWaitMS;
            _pattern = pattern;
            _fadeSteps = fadeSteps;
            _maxPatternsVisible = maxPatternsVisible;
            _numberOfFadeCycles = numberOfFadeCycles;
        }

        public List<Frame> Create()
        {
            Color[][] fadePatterns = FadeCalculation.CalculateFadedPatterns(_pattern, _fadeSteps);

            Random random = new Random();
            List<FadingItem> fadeItems = new List<FadingItem>();

            // Initial frame. Create a random number of FadeItems.
            AddFadeItemsThatDoNotOverlap(fadeItems,random);

            // Start drawing
            List<Frame> frames = new List<Frame>();
            for (int i = 0; i < _numberOfFadeCycles; i++)
            {
                DrawFadeCycle(frames, fadeItems, fadePatterns, random);
            }

            return frames;
        }

        private void DrawFadeCycle(List<Frame> frames, List<FadingItem> existingFadeItems, Color[][] fadePatterns, Random random)
        {
            for (int i = 0; i < _fadeSteps; i++)
            {
                int frameTimeStampRelative = frames.Count * _frameWaitMS;
                Frame frame = DrawFadeStep(frames.Count, frameTimeStampRelative, existingFadeItems, fadePatterns, out List<int> fadeItemIndexesToRemove);
                frames.Add(frame);

                if (fadeItemIndexesToRemove.Count > 0)
                {
                    // Remove items that have passed
                    existingFadeItems = CleanUpFadeItemsThatAreDone(existingFadeItems, fadeItemIndexesToRemove);
                }

                // Add new items
                if (random.Next(0, 2) == 1)
                {
                    AddFadeItemsThatDoNotOverlap(existingFadeItems,random);
                }
            }
        }

        private Frame DrawFadeStep( int frameIndex, int frameTimeStampRelative, List<FadingItem> existingFadeItems, Color[][] fadePatterns, out List<int> fadeItemIndexesToRemove)
        {
            Color clearColor = Color.FromArgb(0, 0, 0);
            fadeItemIndexesToRemove = new List<int>();
            
            Frame frame = new Frame(frameIndex, frameTimeStampRelative);

            for (int j = 0; j < existingFadeItems.Count; j++)
            {
                FadingItem item = existingFadeItems[j];
                if (item.FramesDisplayed == _fadeSteps * 2)
                {
                    // The item has displayed it last fade pattern and should be cleared.
                    for (int k = 0; k < _pattern.Length; k++)
                    {
                        int startPositionOnLedStrip = item.StartPosition + k;
                        // check if not out of bounds
                        if (startPositionOnLedStrip > _stripLength - 1)
                        {
                            break;
                        }
                        frame.Add(new PixelInstruction((uint)startPositionOnLedStrip, clearColor));
                    }
                    fadeItemIndexesToRemove.Add(j);
                    continue;
                }


                // get the fade pattern
                int fadePatternIndex;
                if (item.FramesDisplayed < _fadeSteps)
                {
                    fadePatternIndex = item.FramesDisplayed;
                }
                else
                {
                    fadePatternIndex = _fadeSteps - (item.FramesDisplayed - _fadeSteps) -1;
                } 
               
                Color[] fadePattern = fadePatterns[fadePatternIndex];
                for (int k = 0; k < _pattern.Length; k++)
                {
                    int startPositionOnLedStrip = item.StartPosition + k;
                    // check if not out of bounds
                    if (startPositionOnLedStrip > _stripLength - 1)
                    {
                        break;
                    }

                    frame.Add(new PixelInstruction((uint)startPositionOnLedStrip, fadePattern[k]));
                }

                item.FramesDisplayed++;
            }

            return frame;
        }

        private List<FadingItem> CleanUpFadeItemsThatAreDone(List<FadingItem> existingFadeItems, List<int> fadeItemIndexesToRemove)
        {
            if (fadeItemIndexesToRemove.Count == 0)
            {
                return existingFadeItems;
            }

            if (fadeItemIndexesToRemove.Count == 1)
            {
                existingFadeItems.RemoveAt(fadeItemIndexesToRemove[0]);
                return existingFadeItems;
            }

            List<FadingItem> newItems = new List<FadingItem>();
            int removeIndex = 0;
            for (int i = 0; i < existingFadeItems.Count; i++)
            {
                if (fadeItemIndexesToRemove.Count > removeIndex)
                {
                    if (i == fadeItemIndexesToRemove[removeIndex])
                    {
                        removeIndex++;
                        continue;
                    }
                }
                
                newItems.Add(existingFadeItems[i]);
                    
            }

            return newItems;
        }

        private void AddFadeItemsThatDoNotOverlap(List<FadingItem> existingFadeItems, Random random)
        {
            int maxAttempts = 100;
            int numberOfItemsToAdd = random.Next(0, _maxPatternsVisible - existingFadeItems.Count + 1);
            int numberOfAddedItems = 0;

            while (maxAttempts-- > 0 && numberOfAddedItems != numberOfItemsToAdd)
            {
                int startPos = random.Next(0, _stripLength);
                int endPos = startPos + _pattern.Length;
                // Check if an FadeItem is already present on this position
                if (ItemsDoNotOverlap(startPos, endPos, existingFadeItems))
                {
                    existingFadeItems.Add(new FadingItem(startPos));
                    numberOfAddedItems++;
                }
            }
        }

        private bool ItemsDoNotOverlap(int startPos, int endPos, List<FadingItem> items)
        {
            foreach (FadingItem fadeItem in items)
            {
                int itemStart = fadeItem.StartPosition;
                int itemStop = itemStart + _pattern.Length;

                if (startPos <= itemStop && itemStart <= endPos)
                {
                    return false;
                }
            }
            return true;
        }
    }

    internal class FadingItem
    {
        public int StartPosition { get; }
        public int FramesDisplayed { get; set; }
        
        public FadingItem(int startPosition)
        {
            StartPosition = startPosition;
            FramesDisplayed = 0;
        }
    }
}
