using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServerLib.Bpm;

public class BpmRecorder : ReactiveObject
{
    private List<long> intervals = new List<long>(100);

    [Reactive] public double Bpm { get; set; }
    [Reactive] public long Interval { get; set; }
    public List<long> Measurements { get; set; } = new List<long>(100);


    public void OnNextBeat(long clickedAt)
    {
        AddMeasurement(clickedAt);
        if (Measurements.Count > 1)
        {
            long averageInterval = GetAverageIntervalInMilliseconds();
            Interval = averageInterval;
            Bpm = ((1000.0d / averageInterval) * 60);
        }
    }

    private void AddMeasurement(long clickedAt)
    {
        if (Measurements.Count < 1)
        {
            Measurements.Add(clickedAt);
            return;
        }

        long previousMeasurement = Measurements.Last();
        long interval = clickedAt - previousMeasurement;
        intervals.Add(interval);
        Measurements.Add(clickedAt);
    }

    private long GetAverageIntervalInMilliseconds()
    {
        return intervals.Sum() / intervals.Count;
    }
}