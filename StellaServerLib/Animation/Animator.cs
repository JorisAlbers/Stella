using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Animation
{
    public class Animator : IAnimator
    {
        private readonly IEnumerator<Frame> frameEnumerator;
        private readonly int[] _stripLengthPerPi;
        private readonly List<PiMaskItem> _mask;
        private readonly int _numberOfPis;
        private readonly AnimationTransformation[] _animationTransformations;

        /// <summary>
        /// CTOR
        /// </summary>s
        /// <param name="frameProvider">The drawer to get frames from.</param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="animationTransformations">Used by the drawers as input to draw frames with</param>
        public Animator(IFrameProvider frameProvider, int[] stripLengthPerPi, List<PiMaskItem> mask, AnimationTransformation[] animationTransformations)
        {
            frameEnumerator = frameProvider.GetEnumerator();
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            _animationTransformations = animationTransformations;
            _numberOfPis = _stripLengthPerPi.Length;
        }

        public void SetFrameWaitMs(int frameWaitMs)
        {
            if (frameWaitMs < 1)
            {
                throw new ArgumentException($"The frameWaitMs must be at least 1 ms.");
            }
            
            for (int i = 0; i < _animationTransformations.Length; i++)
            {
                _animationTransformations[i].FrameWaitMs = frameWaitMs;
            }
        }

        public void SetFrameWaitMs(int frameWaitMs, int animationIndex)
        {
            if (frameWaitMs < 10)
            {
                throw new ArgumentException($"The frameWaitMs must be at least 10 ms.");
            }

            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            _animationTransformations[animationIndex].FrameWaitMs = frameWaitMs;
        }

        public int GetFrameWaitMs(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            return _animationTransformations[animationIndex].FrameWaitMs;
        }

        public float GetBrightnessCorrection(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            return _animationTransformations[animationIndex].BrightnessCorrection;
        }

        public void SetBrightnessCorrection(float brightnessCorrection)
        {
            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException("The brightness correction must be between -1 and 1");
            }

            for (int i = 0; i < _animationTransformations.Length; i++)
            {
                _animationTransformations[i].BrightnessCorrection = brightnessCorrection;
            }
        }

        public void SetBrightnessCorrection(int animationIndex, float brightnessCorrection)
        {
            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            if (brightnessCorrection < -1 || brightnessCorrection > 1)
            {
                throw new ArgumentException("The brightness correction must be between -1 and 1");
            }

            _animationTransformations[animationIndex].BrightnessCorrection = brightnessCorrection;
        }

        public float[] GetRgbFadeCorrection(int animationIndex)
        {
            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            return _animationTransformations[animationIndex].RgbFadeCorrection;
        }

        public void SetRgbFadeCorrection(float[] rgbFadeCorrection)
        {
            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            for (int i = 0; i < _animationTransformations.Length; i++)
            {
                _animationTransformations[i].RgbFadeCorrection = rgbFadeCorrection;
            }
        }

        public void SetRgbFadeCorrection(int animationIndex, float[] rgbFadeCorrection)
        {
            if (animationIndex < 0 || animationIndex >= _animationTransformations.Length)
            {
                throw new ArgumentException($"There is no animation at index {animationIndex}");
            }

            if (rgbFadeCorrection.Any(x => x > 0 || x < -1))
            {
                throw new ArgumentException($"The rgb corrections must be between -1 and 0 ");
            }

            for (int i = 0; i < _animationTransformations.Length; i++)
            {
                _animationTransformations[i].RgbFadeCorrection = rgbFadeCorrection;
            }
        }

        /// <inheritdoc />
        public FrameWithoutDelta[] GetNextFramePerPi()
        {
            frameEnumerator.MoveNext();
            Frame combinedFrame = frameEnumerator.Current;
            return SplitFrameOverPis(combinedFrame, _mask);
        }

        private FrameWithoutDelta[] SplitFrameOverPis(Frame combinedFrame, List<PiMaskItem> mask)
        {
            // Create a new Frame for each pi
            Frame[] framePerPi = new Frame[_numberOfPis];
            for (int i = 0; i < _numberOfPis; i++)
            {
                framePerPi[i] = new Frame(combinedFrame.Index, combinedFrame.TimeStampRelative);
            }

            // Disperse the instructions over each pi frame
            for (int i = 0; i < combinedFrame.Count; i++)
            {
                PiMaskItem maskItem = mask[(int)combinedFrame[i].Index];
                framePerPi[maskItem.PiIndex].Add(new PixelInstruction(maskItem.PixelIndex, combinedFrame[i].Color));
            }

            // Convert to non-delta
            FrameWithoutDelta[] frameWithoutDeltaPerPi = new FrameWithoutDelta[_numberOfPis];
            for (int i = 0; i < framePerPi.Length; i++)
            {
                if (framePerPi[i].Count > 0)
                {
                    frameWithoutDeltaPerPi[i] = new FrameWithoutDelta(combinedFrame.Index, combinedFrame.TimeStampRelative, _stripLengthPerPi[i]);
                    foreach (PixelInstruction pixelInstruction in framePerPi[i])
                    {
                        frameWithoutDeltaPerPi[i][pixelInstruction.Index] = new PixelInstructionWithoutDelta(pixelInstruction.Color);
                    }
                }
            }

            return frameWithoutDeltaPerPi;
        }
    }
}
