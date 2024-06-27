using System;
using System.Threading;
using StellaServerLib;
using StellaServerLib.Animation.Transformation;

namespace StellaVisualizer.Server;

public class BpmAnimationTransformer : IDisposable
{
    private readonly long _interval;
    private Timer _timer;
    private long _nextBeatAt;
    private long _waitTime;
    private readonly StellaServer _stellaServer;

    public EventHandler OnBeat;

    public BpmAnimationTransformer(long interval, long waitTime,
        StellaServer stellaServer)
    {
        _interval = interval;
        _waitTime = waitTime;
        _stellaServer = stellaServer;
    }

    public void Run(long nextBeatAt)
    {
        if (_timer != null)
        {
            throw new Exception("This BPM animator already started!");
        }

        _nextBeatAt = nextBeatAt + _interval; // add one beat for some buffer

        // TODO implement Wait time  to add some buffer
        long timer1Start = _nextBeatAt - Environment.TickCount;
        _timer = new Timer(Callback1, null, timer1Start, _interval);
    }

    private bool _toggle;

    private void Callback1(object? state)
    {
        long nextAt = _nextBeatAt;
        _nextBeatAt = nextAt + _interval;

        // TODO implement waittime as buffer
        /*while (Environment.TickCount < nextAt)
        {
        }*/

        if (_toggle)
        {
            _toggle = false;
            _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(-0.8f);
        }
        else
        {
            _toggle = true;
            _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(0);
        }

        var handler = OnBeat;
        if (handler != null)
        {

            OnBeat.Invoke(this, EventArgs.Empty);
        }
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}