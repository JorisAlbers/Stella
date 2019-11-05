using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public class Animator : IAnimator
    {
        private readonly IFrameProvider _frameProvider;
        private readonly PlayList _playList;
        private readonly IFrameProviderCreator _frameProviderCreator;
        private readonly List<PiMaskItem> _mask;
        private readonly int _numberOfPis;

        private readonly PixelInstruction[][] _currentFrame;
        public TransformationController TransformationController { get; private set; }

        /// <summary>
        /// CTOR
        /// </summary>s
        /// <param name="playList"></param>
        /// <param name="frameProviderCreator"></param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="masterTransformationSettings"></param>
        public Animator(PlayList playList, IFrameProviderCreator frameProviderCreator, int[] stripLengthPerPi, List<PiMaskItem> mask, TransformationSettings masterTransformationSettings)
        {
            _playList = playList;
            _frameProviderCreator = frameProviderCreator;
            _mask = mask;
            _numberOfPis = stripLengthPerPi.Length;

            _currentFrame = new PixelInstruction[stripLengthPerPi.Sum()][];
            for (int i = 0; i < stripLengthPerPi.Length; i++)
            {
                _currentFrame[i] = new PixelInstruction[stripLengthPerPi[i]];
            }

            if (playList.Items.Length == 1)
            {
                // There is just a single item in the playlist. This item will always be on display.
                _frameProvider = frameProviderCreator.Create(playList.Items[0].Storyboard, masterTransformationSettings, out TransformationController controller);
                TransformationController = controller;
            }
            else
            {
                // Loop through the items.
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public FrameWithoutDelta[] GetNextFramePerPi()
        {
            // Get the combined frame from the FrameProvider
            _frameProvider.MoveNext();
            Frame combinedFrame = _frameProvider.Current;

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
                PixelInstructionWithDelta instructionWithDelta = combinedFrame[i];

                PiMaskItem maskItem = mask[instructionWithDelta.Index];
                framePerPi[maskItem.PiIndex].Add(new PixelInstructionWithDelta(maskItem.PixelIndex, instructionWithDelta.R, instructionWithDelta.G,instructionWithDelta.B));
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
                    foreach (PixelInstructionWithDelta pixelInstruction in framePerPi[index])
                    {
                        _currentFrame[index][pixelInstruction.Index] = new PixelInstruction(pixelInstruction.R, pixelInstruction.G, pixelInstruction.B);
                    }

                    // Copy all frames from the current frame to the frameWithoutDelta
                    overlayFramePerPi[index] = new FrameWithoutDelta(frame.Index, frame.TimeStampRelative, _currentFrame[index].Length);
                    for (int i = 0; i < _currentFrame[index].Length; i++)
                    {
                        PixelInstruction instruction = _currentFrame[index][i];
                        overlayFramePerPi[index][i] = new PixelInstruction(instruction.R, instruction.G, instruction.B);
                    }
                    
                }
            }

            return overlayFramePerPi;
        }
    }
}
