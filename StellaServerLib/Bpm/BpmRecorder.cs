using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServerLib.Bpm;

public class BpmRecorder : ReactiveObject
{
    private List<long> measurements = new List<long>(100);
    private List<long> intervals = new List<long>(100);

    [Reactive] public double Bpm { get; set; }
    [Reactive] public long Interval { get; set; }
    

    public void OnNextBeat(long clickedAt)
    {
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
}