using System;
using System.Collections.Generic;
using DynamicData;
using ReactiveUI;
using StellaServer.Midi;

namespace StellaServer
{
    public class MidiPanelViewModel : ReactiveObject
    {
        public int Rows { get; }
        public int Columns { get; }


        public MidiPadButtonViewModel[] Pads { get; }


        public MidiPanelViewModel(int rows, int columns, int controllerStartIndex, MidiInputManager midiInputManager)
        {
            Rows = rows;
            Columns = columns;
            var pads = new List<MidiPadButtonViewModel>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pads.Add(new MidiPadButtonViewModel(controllerStartIndex + (columns * i) + i ));
                }
            }

            Pads = pads.ToArray();
        }
    }

    public class MidiPadButtonViewModel : ReactiveObject
    {
        private readonly int _controller;

        public MidiPadButtonViewModel(int controller)
        {
            _controller = controller;
        }
    }
}