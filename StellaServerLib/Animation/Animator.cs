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

        private readonly PixelInstructionWithoutDelta[][] _currentFrame;
        public TransformationController TransformationController { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>s
        /// <param name="frameProvider">The drawer to get frames from.</param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="transformationController">Class that provides run time animation changes</param>
        public Animator(IFrameProvider frameProvider, int[] stripLengthPerPi, List<PiMaskItem> mask, TransformationController transformationController)
        {
            frameEnumerator = frameProvider.GetEnumerator();
            _stripLengthPerPi = stripLengthPerPi;
            _mask = mask;
            TransformationController = transformationController;
            _numberOfPis = _stripLengthPerPi.Length;

            _currentFrame = new PixelInstructionWithoutDelta[stripLengthPerPi.Sum()][];
            for (int i = 0; i < stripLengthPerPi.Length; i++)
            {
                _currentFrame[i] = new PixelInstructionWithoutDelta[stripLengthPerPi[i]];
            }
        }

        /// <inheritdoc />
        public FrameWithoutDelta[] GetNextFramePerPi()
        {
            // Get the combined frame from the FrameProvider
            frameEnumerator.MoveNext();
            Frame combinedFrame = frameEnumerator.Current;

            // Split the frame over pis
            Frame[] framePerPi = SplitFrameOverPis(combinedFrame, _mask);
            
            // Overlay with the previous frame
            return OverlayWithCurrentFrame(framePerPi);
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
                PixelInstruction instruction = combinedFrame[i];

                PiMaskItem maskItem = mask[(int)instruction.Index];
                framePerPi[maskItem.PiIndex].Add(new PixelInstruction(maskItem.PixelIndex, instruction.R, instruction.G,instruction.B));
            }

            return framePerPi;
        }

        private FrameWithoutDelta[] OverlayWithCurrentFrame(Frame[] framePerPi)
        {
            FrameWithoutDelta[] overlayFramePerPi = new FrameWithoutDelta[framePerPi.Length];
            for (int index = 0; index < framePerPi.Length; index++)
            {
                Frame frame = framePerPi[index];
                if (frame != null && frame.Count > 0)
                {
                    // Overlay in current frame 
                    foreach (PixelInstruction pixelInstruction in framePerPi[index])
                    {
                        _currentFrame[index][pixelInstruction.Index] = new PixelInstructionWithoutDelta(pixelInstruction.R, pixelInstruction.G, pixelInstruction.B);
                    }

                    // Copy all frames from the current frame to the frameWithoutDelta
                    overlayFramePerPi[index] = new FrameWithoutDelta(frame.Index, frame.TimeStampRelative, _currentFrame[index].Length);
                    for (int i = 0; i < _currentFrame[index].Length; i++)
                    {
                        PixelInstructionWithoutDelta instruction = _currentFrame[index][i];
                        overlayFramePerPi[index][i] = new PixelInstructionWithoutDelta(instruction.R, instruction.G, instruction.B);
                    }
                    
                }
            }

            return overlayFramePerPi;
        }
    }
}
