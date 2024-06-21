using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace StellaVisualizer.Server;

public class BpmViewModel : INotifyPropertyChanged
{
    private double _bpm;
    private long _interval;
    private readonly BpmRecorder _bpmRecorder;
    public event PropertyChangedEventHandler PropertyChanged;


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

    public void OnNextBeat()
    {
        _bpmRecorder.OnNextBeat();
    }
}