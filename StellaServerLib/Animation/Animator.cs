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

        /// <summary>+
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
                        _startAtTicks = Environment.TickCount;
                        _ticksPaused = 0;
                        _currentPauseStarted = 0;
                        _frameProvider = nextFrameProvider;
                        StoryboardTransformationController = controller;

                        i = (i + 1 ) % playList.Items.Length;
                    }
                });
            }
        }

        private FrameWithoutDelta[] _preparedFramePerClient;
        private long _startAtTicks;

        private long _ticksPaused = 0;
        private long _currentPauseStarted = 0;

        public bool TryGetFramePerClient(out FrameWithoutDelta[] frames)
        {
            if (StoryboardTransformationController.Settings.MasterSettings.IsPaused)
            {
                if (_currentPauseStarted == 0)
                {
                    _currentPauseStarted = Environment.TickCount;
                }
                
                frames = null; // TODO adjust timestamprelative
                return false;
            }

            // Adjust the time paused, this will make sure the next frames after the pause have more or less the correct timing.
            // TODO instead of using a timestamp relative to the start of the animation, use a timestamp relative to the previous frame.
            if (_currentPauseStarted != 0)
            {
                _ticksPaused += Environment.TickCount - _currentPauseStarted;
                _currentPauseStarted = 0;
            }

            IFrameProvider frameProvider = _frameProvider;
            if (frameProvider == null) // animations are off. 
            {
                frames = null;
                return false;
            }

            if (frameProvider.Current == null || // No frame was prepared.
                _preparedFramePerClient == null) // The previous frame was consumed
            {
                frameProvider.MoveNext();
                if (frameProvider.Current == null) // Give up. Maybe at a later time the provider has frames available.
                {
                    frames = null;
                    return false;
                }
                
                // Separate frame over clients
                _preparedFramePerClient = GetFrames(frameProvider.Current);
            }

            // Check if we should display the frame now.
            long now = Environment.TickCount;
            long renderNextFrameAt = _startAtTicks + _ticksPaused + frameProvider.Current.TimeStampRelative;
            if (now < renderNextFrameAt)
            {
                // render will happen in other loop
                frames = null;
                return false;
            }
            
            // We need to draw NOW!
            frames = _preparedFramePerClient;
            _preparedFramePerClient = null;
            return true;
        }

        public void StartAnimation(int startAt)
        {
            _startAtTicks = startAt;
        }

        private FrameWithoutDelta[] GetFrames(Frame frame)
        {
            // Split the frame over pis
            Frame[] framePerPi = SplitFrameOverPis(frame, _mask);

            // Overlay with the previous frame
            FrameWithoutDelta[] frames = OverlayWithCurrentFrame(framePerPi);
            
            return frames;
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
