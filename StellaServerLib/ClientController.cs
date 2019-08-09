using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaLib.Network;
using StellaLib.Network.Protocol;
using StellaLib.Network.Protocol.Animation;
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

        public ClientController(IServer server)
        {
            _server = server;
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
                    frames = animationWithStartingTime.Animator.GetNextFramePerPi();
                    renderNextFrameAt = animationWithStartingTime.StartAt.Ticks + frames.First(x => x != null).TimeStampRelative * TimeSpan.TicksPerMillisecond;
                }

                long now = DateTime.Now.Ticks;

                if (now < renderNextFrameAt)
                {
                    // render will happen in other loop
                    continue;
                }

                // Render
                SendRenderFrame(frames);

                // Prepare
                frames = animationWithStartingTime.Animator.GetNextFramePerPi();
                renderNextFrameAt = animationWithStartingTime.StartAt.Ticks + frames.First(x => x != null).TimeStampRelative * TimeSpan.TicksPerMillisecond;
            }
        }

        private void SendRenderFrame(FrameWithoutDelta[] frames)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] != null)
                {
                    _server.SendToClient(i,frames[i]);
                }
            }
        }

        public void StartAnimation(IAnimator animator, DateTime at)
        {
            lock(_frameSetLock)
            {
                _animationWithStartingTime = new AnimationWithStartingTime(animator, at);
            }
        }
        
        public void Dispose()
        {
            _isDisposed = true;
        }
    }

    internal class AnimationWithStartingTime
    {
        public IAnimator Animator { get; }
        public DateTime StartAt { get; }

        public AnimationWithStartingTime(IAnimator animator, DateTime startAt)
        {
            Animator = animator;
            StartAt = startAt;
        }
    }
}