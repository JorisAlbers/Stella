using System;
using System.Collections.Generic;
using System.Linq;
using StellaLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Animation
{
    public class Animator : IAnimator
    {
        private readonly IEnumerator<Frame> _drawerEnumerator;
        private readonly List<PiMaskItem> _mask;
        private readonly DateTime _startAt;
        private readonly int _numberOfPis;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="drawer">The drawer to get frames from.</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="startAt">The time to start the animation</param>
        public Animator(IDrawer drawer, List<PiMaskItem> mask, DateTime startAt)
        {
            _drawerEnumerator = drawer.GetEnumerator();
            _mask = mask;
            _startAt = startAt;
            _numberOfPis = mask.Select(x => x.PiIndex).Distinct().Count();
        }

        /// <inheritdoc />
        public Frame[] GetNextFramePerPi()
        {
            _drawerEnumerator.MoveNext();
            Frame combinedFrame = _drawerEnumerator.Current;
            return SplitFrameOverPis(combinedFrame, _mask);
        }

        private Frame[] SplitFrameOverPis(Frame combinedFrame, List<PiMaskItem> mask)
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

            return framePerPi;
        }

        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata()
        {
            return new FrameSetMetadata(_startAt);
        }
    }

    public class AnimatorWithoutDelta : IAnimatorWithoutDelta
    {
        private readonly IEnumerator<Frame> _drawerEnumerator;
        private readonly int[] _stripLengthPerPi;
        private readonly List<PiMaskItem> _mask;
        private readonly DateTime _startAt;
        private readonly int _numberOfPis;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="drawer">The drawer to get frames from.</param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="startAt">The time to start the animation</param>
        public AnimatorWithoutDelta(IDrawer drawer, int[] stripLengthPerPi, List<PiMaskItem> mask, DateTime startAt)
        {
            _drawerEnumerator = drawer.GetEnumerator();
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            _startAt = startAt;
            _numberOfPis = _stripLengthPerPi.Length;
        }

        /// <inheritdoc />
        public FrameWithoutDelta[] GetNextFramePerPi()
        {
            _drawerEnumerator.MoveNext();
            Frame combinedFrame = _drawerEnumerator.Current;
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

        /// <inheritdoc />
        public FrameSetMetadata GetFrameSetMetadata()
        {
            return new FrameSetMetadata(_startAt);
        }
    }
}
