using System;
using System.Collections.Generic;
using DynamicData;
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
            var pads = new List<MidiPadButtonViewModel>();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    pads.Add(new MidiPadButtonViewModel(controllerStartIndex + (columns * i) + j ));
                }
            }

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

        [Reactive] public bool KeyDown { get; private set; }

        public MidiPadButtonViewModel(int controller)
        {
            Controller = controller;
        }

        public void PadPressed(PadPressedEvent padPressedEvent)
        {
            KeyDown = padPressedEvent.KeyDown;
        }
    }
}