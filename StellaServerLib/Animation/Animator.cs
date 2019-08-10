using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public class Animator : IAnimator
    {
        private readonly IEnumerator<Frame> frameEnumerator;
        private readonly int[] _stripLengthPerPi;
        private readonly List<PiMaskItem> _mask;
        private readonly int _numberOfPis;
        public AnimationTransformations AnimationTransformations { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>s
        /// <param name="frameProvider">The drawer to get frames from.</param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="animationTransformations">Class that provides run time animation changes</param>
        public Animator(IFrameProvider frameProvider, int[] stripLengthPerPi, List<PiMaskItem> mask, AnimationTransformations animationTransformations)
        {
            frameEnumerator = frameProvider.GetEnumerator();
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            AnimationTransformations = animationTransformations;
            _numberOfPis = _stripLengthPerPi.Length;
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
