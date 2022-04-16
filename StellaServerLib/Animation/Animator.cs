using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaLib.Animation;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Animation.Transformation;

namespace StellaServerLib.Animation
{
    public class Animator : ReactiveObject, IAnimator
    {
        private readonly List<PiMaskItem> _mask;
        private readonly int _numberOfPis;
        private readonly PixelInstruction[][] _currentFrame;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private IFrameProvider _frameProvider;

        [Reactive] public StoryboardTransformationController StoryboardTransformationController { get; private set; }
        public event EventHandler TimeResetRequested;

        /// <summary>
        /// CTOR
        /// </summary>s
        /// <param name="playList"></param>
        /// <param name="frameProviderCreator"></param>
        /// <param name="stripLengthPerPi">The length of the strip of each pi</param>
        /// <param name="mask">The mask to convert the indexes over the pis</param>
        /// <param name="masterAnimationTransformationSettings"></param>
        public Animator(PlayList playList, IFrameProviderCreator frameProviderCreator, int[] stripLengthPerPi, List<PiMaskItem> mask, AnimationTransformationSettings masterAnimationTransformationSettings)
        {
            _mask = mask;
            _numberOfPis = stripLengthPerPi.Length;

            _currentFrame = new PixelInstruction[stripLengthPerPi.Sum()][];
            for (int i = 0; i < stripLengthPerPi.Length; i++)
            {
                _currentFrame[i] = new PixelInstruction[stripLengthPerPi[i]];
            }

            _frameProvider = frameProviderCreator.Create(playList.Items[0].Storyboard, out StoryboardTransformationController controller);
            controller.Init(masterAnimationTransformationSettings);

            // Start the first animation
            StoryboardTransformationController = controller;
            
            if (playList.Items.Length > 1)
            {
                _cancellationTokenSource = new CancellationTokenSource();

                // Start a timer to display the next item
                Task.Run(async () =>
                {
                    int i = 1;
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        // Load the next animation
                        IFrameProvider nextFrameProvider = frameProviderCreator.Create(playList.Items[i].Storyboard, out controller);

                        // Wait
                        await Task.Delay(playList.Items[i].Duration * 1000, _cancellationTokenSource.Token);

                        // Initialize the storyboard controller
                        controller.Init(StoryboardTransformationController.Settings.MasterSettings);

                        // Play
                        // TODO race condition
                        OnTimeResetRequested();
                        _frameProvider = nextFrameProvider;
                        StoryboardTransformationController = controller;

                        i = (i + 1 ) % playList.Items.Length;
                    }
                });
            }
        }

        
        public bool TryPeek(ref FrameMetadata previous)
        {
            if (StoryboardTransformationController.Settings.MasterSettings.IsPaused)
            {
                previous = null;
                return false;
            }

            IFrameProvider frameProvider = _frameProvider;
            if (frameProvider == null) // animations are off. 
            {
                previous = null;
                return false;
            }

            if (previous == null) // Caller does not have a frame yet
            {
                frameProvider.MoveNext();
            }

            Frame frame = frameProvider.Current;
            if (frame ==  null)
            {
                previous = null;
                return false;
            }

            if(previous == null ||  // Caller does not have a frame yet
               frame.Index != previous.FrameIndex || // Caller has an outdated frame
               frame.TimeStampRelative != previous.TimeStampRelative) 
            {
                // Caller is no longer looking at the same frame.
                // Calculate new one.
                previous = new FrameMetadata()
                {
                    FrameIndex = frame.Index, 
                    TimeStampRelative = frame.TimeStampRelative,
                    Frames = GetFrames(frame, frameProvider)
                };
                return true;
            }

            //  Caller is still looking at the same frame.
            return true;
        }

        private FrameWithoutDelta[] GetFrames(Frame frame, IFrameProvider frameProvider)
        {
            // Split the frame over pis
            Frame[] framePerPi = SplitFrameOverPis(frame, _mask);

            // Overlay with the previous frame
            FrameWithoutDelta[] frames = OverlayWithCurrentFrame(framePerPi);
            
            return frames;
        }

        private void OnTimeResetRequested()
        {
            TimeResetRequested?.Invoke(this,new EventArgs());
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

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
        }
    }

    public class FrameMetadata
    {
        public int FrameIndex;
        public long TimeStampRelative;
        public FrameWithoutDelta[] Frames;
    }
}
