using System;
using System.Collections.Generic;
using System.Linq;
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
        private IAnimator _animator;
        private object _frameSetLock = new object();
        private bool _isDisposed;

        private FrameSetMetadata _frameSetMetadata;

        private List<Frame[]> _framesPerPi;

        public ClientController(IServer server)
        {
            _server = server;
        }

        public async void Run()
        {
            await new TaskFactory().StartNew(MainLoop);
        }

        private void MainLoop()
        {
            FrameWithoutDelta[] frames = null;
            long renderNextFrameAt = 0;

            while (!_isDisposed)
            {
                IAnimator animator = _animator;
                if (animator == null)
                {
                    // No animation on display.
                    renderNextFrameAt = 0;
                    continue;
                }

                // Prepare if the animation is about to start
                if (renderNextFrameAt == 0)
                {
                    frames = _animator.GetNextFramePerPi();
                    renderNextFrameAt = _frameSetMetadata.TimeStamp.Ticks + frames.First(x => x != null).TimeStampRelative * TimeSpan.TicksPerMillisecond;
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
                frames = animator.GetNextFramePerPi();
                renderNextFrameAt = _frameSetMetadata.TimeStamp.Ticks + frames.First(x => x != null).TimeStampRelative * TimeSpan.TicksPerMillisecond;
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
                _animator = animator;
                _frameSetMetadata = new FrameSetMetadata(at);
            }
        }
        
        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}