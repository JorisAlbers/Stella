using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace StellaVisualizer.Server;

public class BpmRecorder : INotifyPropertyChanged
{
    private List<long> measurements = new List<long>(100);
    private List<long> intervals = new List<long>(100);
    private double _bpm;
    private long _interval;

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


    public void OnNextBeat()
    {
        long clickedAt = Environment.TickCount;
        AddMeasurement(clickedAt);
        if (measurements.Count > 1)
        {
            long averageInterval = GetAverageIntervalInMilliseconds();
            Interval = averageInterval;
            Bpm = ((1000.0d / averageInterval) * 60);
        }
    }

    private void AddMeasurement(long clickedAt)
    {
        if (measurements.Count < 1)
        {
            measurements.Add(clickedAt);
            return;
        }

        long previousMeasurement = measurements.Last();
        long interval = clickedAt - previousMeasurement;
        intervals.Add(interval);
        measurements.Add(clickedAt);
    }

    private long GetAverageIntervalInMilliseconds()
    {
        return intervals.Sum() / intervals.Count;
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}