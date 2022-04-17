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
        private IAnimator _animator;

        public ClientController(IServer server, int maximumFrameRate, int numberOfClients)
        {
            _server = server;
            _minimumTicksPerFrame = 1000 / maximumFrameRate;
            _nextRenderAllowedAtperPi = new long[numberOfClients];
        }

        public void Run()
        {
            Thread thread = new Thread(MainLoop) {Priority = ThreadPriority.Highest};
            thread.Start();
        }

        //  

        private void MainLoop()
        {
            while (!_isDisposed)
            {
                // Check if there is an animator
                IAnimator animator = _animator;
                if (animator == null)
                {
                    continue;
                }

                // Check if there is a frame available.
                if (!animator.TryGetFramePerClient(out FrameWithoutDelta[] frames))
                {
                    continue;
                }
                
                // Render.
                SendRenderFrame(frames);
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
        public void StartAnimation(IAnimator animator)
        {
            animator.StartAnimation(Environment.TickCount);
            _animator = animator;
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