using System;
using System.Linq;
using System.Threading;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaServerLib
{
    public class ClientController : IDisposable
    {
        private readonly IServer _server;
        private AnimationWithStartingTime _animationWithStartingTime;
        private object _frameSetLock = new object();
        private bool _isDisposed;

        private long[] _nextRenderAllowedAtperPi;
        private long _minimumTicksPerFrame;

        public ClientController(IServer server, int maximumFrameRate)
        {
            _server = server;
            _minimumTicksPerFrame = 1000 / maximumFrameRate;
        }

        public void Run()
        {
            Thread thread = new Thread(MainLoop) {Priority = ThreadPriority.Highest};
            thread.Start();
        }

        //  

        private void MainLoop()
        {
            FrameMetadata frameMetadata = null;
            while (!_isDisposed)
            {
                AnimationWithStartingTime animationWithStartingTime = _animationWithStartingTime;
                if (animationWithStartingTime == null)
                {
                    // No animation on display.
                    continue;
                }

                // Check if there is a frame available
                if (!animationWithStartingTime.Animator.TryPeek(ref frameMetadata))
                {
                    continue;
                }


                // Check if we should display the frame now.
                long now = Environment.TickCount;
                long renderNextFrameAt = animationWithStartingTime.StartAtTicks + frameMetadata.TimeStampRelative;
                if (now < renderNextFrameAt)
                {
                    // render will happen in other loop
                    continue;
                }

                // Render.
                if (_nextRenderAllowedAtperPi == null)
                {
                    _nextRenderAllowedAtperPi = new long[frameMetadata.Frames.Length]; // TODO insert number of clients and initialize this array at ctor
                }

                SendRenderFrame(frameMetadata.Frames);
                frameMetadata = null; // set back to null so the animator will calculate the next frame.
            }
        }

        private void SendRenderFrame(FrameWithoutDelta[] frames)
        {
            int now = Environment.TickCount;
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] != null)
                {
                    if (now >= _nextRenderAllowedAtperPi[i])
                    {
                        _server.SendToClient(i, frames[i]);
                        _nextRenderAllowedAtperPi[i] = now + _minimumTicksPerFrame;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="animator">The animator to retrieve frames from</param>
        /// <param name="startAtTicks">The environment.TickCount at which to start the animation</param>
        public void StartAnimation(IAnimator animator, long startAtTicks)
        {
            lock(_frameSetLock)
            {
                if (_animationWithStartingTime != null)
                {
                    _animationWithStartingTime.Animator.TimeResetRequested -= AnimatorOnTimeResetRequested;
                }

                animator.TimeResetRequested += AnimatorOnTimeResetRequested;
                _animationWithStartingTime = new AnimationWithStartingTime(animator, startAtTicks);
            }
        }

        private void AnimatorOnTimeResetRequested(object sender, EventArgs e)
        {
            _animationWithStartingTime = new AnimationWithStartingTime(_animationWithStartingTime.Animator, Environment.TickCount);
        }

        public void Dispose()
        {
            _isDisposed = true;
        }
    }

    internal class AnimationWithStartingTime
    {
        public IAnimator Animator { get; }
        public long StartAtTicks { get; }

        public AnimationWithStartingTime(IAnimator animator, long startAtTicks)
        {
            Animator = animator;
            StartAtTicks = startAtTicks;
        }
    }
}