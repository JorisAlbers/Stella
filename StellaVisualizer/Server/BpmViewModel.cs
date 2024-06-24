using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace StellaVisualizer.Server;

public class BpmViewModel : INotifyPropertyChanged
{
    private double _bpm;
    private long _interval;
    private BpmRecorder _bpmRecorder;
    private BpmAnimationTransformer _bpmAnimationTransformer;
    public event PropertyChangedEventHandler PropertyChanged;
    private bool _animationToggle;



    public double Bpm
    {
        get => _bpm;
        set
        {
            if (value.Equals(_bpm)) return;
            _bpm = value;
            OnPropertyChanged();
        }
    }

    public long Interval
    {
        get => _interval;
        set
        {
            if (value == _interval) return;
            _interval = value;
            OnPropertyChanged();
        }
    }

    public bool AnimationToggle
    {
        get => _animationToggle;
        set
        {
            if (value == _animationToggle) return;
            _animationToggle = value;
            OnPropertyChanged();
        }
    }

    public BpmViewModel()
    {
        _bpmRecorder = new BpmRecorder();
        _bpmRecorder.PropertyChanged += BpmRecorderOnPropertyChanged;
    }

    private void BpmRecorderOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(BpmRecorder.Bpm):
                Bpm = _bpmRecorder.Bpm;
                break;
            case nameof(BpmRecorder.Interval):
                Interval = _bpmRecorder.Interval;
                break;
        }
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    
    public void OnNextBeat()
    {
        _bpmRecorder.OnNextBeat();
    }

    public void Start()
    {
        long interval = _bpmRecorder.Interval;
        long nextAt = Environment.TickCount + interval; // TODO use previous measurements to more accurately set the beat.
        _bpmAnimationTransformer = new BpmAnimationTransformer(_bpmRecorder.Interval, 50);
        _bpmAnimationTransformer.OnBeat += OnBeat;
        _bpmAnimationTransformer.Run(nextAt);
    }

    private void OnBeat(object sender, EventArgs e)
    {
        AnimationToggle = !AnimationToggle;
    }

    public void Stop()
    {
        _bpmAnimationTransformer.Dispose();
        _bpmAnimationTransformer.OnBeat -= OnBeat;
        _bpmAnimationTransformer = null;
        _bpmRecorder.PropertyChanged -= BpmRecorderOnPropertyChanged;

        _bpmRecorder = new BpmRecorder();
        _bpmRecorder.PropertyChanged += BpmRecorderOnPropertyChanged;
        Bpm = 0;
        Interval = 0;
    }
}

public class ActionCommand : ICommand
{
    private readonly Action _action;

    public ActionCommand(Action action)
    {
        _action = action;
    }

    public void Execute(object parameter)
    {
        _action();
    }

    public bool CanExecute(object parameter)
    {
        return true;
    }

    public event EventHandler CanExecuteChanged;
}