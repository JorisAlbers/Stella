using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StellaVisualizer.Server;

public class BpmViewModel : INotifyPropertyChanged
{
    private double _bpm;
    private long _interval;
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