using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DynamicData;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Midi;

namespace StellaServer
{
    public class MidiPanelViewModel : ReactiveObject
    {
        private readonly MidiInputManager _midiInputManager;
        public int Rows { get; }
        public int Columns { get; }


        public MidiPadButtonViewModel[] Pads { get; }


        public MidiPanelViewModel(int rows, int columns, int controllerStartIndex, MidiInputManager midiInputManager)
        {
            _midiInputManager = midiInputManager;
            Rows = rows;
            Columns = columns;
            var pads = new List<MidiPadButtonViewModel>
            {
                new MidiPadButtonViewModel(controllerStartIndex + 0, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 1, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 2, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 3, MdiPadMode.StartAnimation),

                new MidiPadButtonViewModel(controllerStartIndex + 4, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 5, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 6, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 7, MdiPadMode.StartAnimation),

                new MidiPadButtonViewModel(controllerStartIndex + 8, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 9, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 10, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 11, MdiPadMode.StartAnimation),

                new MidiPadButtonViewModel(controllerStartIndex + 12, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 13, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 14, MdiPadMode.StartAnimation),
                new MidiPadButtonViewModel(controllerStartIndex + 15, MdiPadMode.BpmMeasurement),
            };
            
            Pads = pads.ToArray();

            midiInputManager.PadPressed.Subscribe(x =>
            {
                Pads[x.ControllerIndex - controllerStartIndex].PadPressed(x);
            });

        }
    }

    public class MidiPadButtonViewModel : ReactiveObject
    {
        public int Controller { get; }
        public MdiPadMode Mode { get; }

        [Reactive] public double Bpm { get; set; }

        [Reactive] public bool KeyDown { get; private set; }

        public MidiPadButtonViewModel(int controller, MdiPadMode mode)
        {
            Controller = controller;
            Mode = mode;
        }

        private Stopwatch _stopwatch;
        private List<TimeSpan> _measurements = new List<TimeSpan>();

        public void PadPressed(PadPressedEvent padPressedEvent)
        {
            KeyDown = padPressedEvent.KeyDown;

            if (Mode == MdiPadMode.BpmMeasurement && KeyDown)
            {
                if (_stopwatch == null)
                {
                    _stopwatch = new Stopwatch();
                    _stopwatch.Start();
                    return;
                }

                TimeSpan elapsed = _stopwatch.Elapsed;
                _stopwatch.Reset();
                _stopwatch.Start();

                if (elapsed.Seconds > 2)
                {
                    _measurements = new List<TimeSpan>();
                    Bpm = 0;
                    return;
                }

                _measurements.Add(elapsed);

                if (_measurements.Count > 0)
                {
                    Bpm = Math.Round(CalculateBpm(_measurements), 3);
                }
            }

        }

        private double CalculateBpm(List<TimeSpan> measurements)
        {
            double total = measurements.Sum(x=>x.Ticks);
            double ticksPerBeat = total / measurements.Count;
            long ticksPerMinute = TimeSpan.TicksPerMinute;

            double bpm = ticksPerMinute / ticksPerBeat;
            return bpm + 1; // first tap should be included.
        }
    }

    public enum MdiPadMode
    {
        NotSet,
        StartAnimation,
        BpmMeasurement,
    }



}