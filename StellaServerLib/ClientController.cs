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

        private void MainLoop()
        {
            FrameWithoutDelta[] frames = null;
            long renderNextFrameAt = 0;

            while (!_isDisposed)
            {
                AnimationWithStartingTime animationWithStartingTime = _animationWithStartingTime;
                if (animationWithStartingTime == null)
                {
                    // No animation on display.
                    renderNextFrameAt = 0;
                    continue;
                }

                // Prepare if the animation is about to start
                if (renderNextFrameAt == 0)
                {
                    animationWithStartingTime.Animator.TryGetNextFramePerPi(out frames);
                    renderNextFrameAt = animationWithStartingTime.StartAtTicks + frames.First(x => x != null).TimeStampRelative;
                    _nextRenderAllowedAtperPi = new long[frames.Length];
                }

                long now = Environment.TickCount;

                if (now < renderNextFrameAt)
                {
                    // render will happen in other loop
                    continue;
                }

                // Render
                SendRenderFrame(frames);

                // Prepare
                animationWithStartingTime.Animator.TryGetNextFramePerPi(out frames);
                renderNextFrameAt = animationWithStartingTime.StartAtTicks + frames.First(x => x != null).TimeStampRelative;
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