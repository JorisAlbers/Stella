using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using ReactiveUI;
using NAudio.Midi;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Midi
{
    public class MidiInputManager : ReactiveObject
    {
        private readonly int _deviceIndex;

        [Reactive] public int RedCorrection { get; set; }



        public MidiInputManager(int deviceIndex)
        {
            _deviceIndex = deviceIndex;
        }

        public void Start()
        {
            var midiIn = new MidiIn(_deviceIndex);
            midiIn.MessageReceived += MidiInOnMessageReceived;
            midiIn.Start();
        }

        private void MidiInOnMessageReceived(object? sender, MidiInMessageEventArgs e)
        {
            switch (e.MidiEvent.CommandCode)
            {
                case MidiCommandCode.ControlChange:
                    HandleControlChangeEvent(e);
                    break;
                default:
                    Console.Out.WriteLine($"MidiInputManager: Received unhandled event code: {e.MidiEvent.CommandCode}");
                    return;
            }
        }

        private void HandleControlChangeEvent(MidiInMessageEventArgs e)
        {
            ControlChangeEvent controlChangeEvent = (ControlChangeEvent)e.MidiEvent;
            int controllerIndex = (int)controlChangeEvent.Controller;

            switch (controllerIndex)
            {
                case 6:
                    float correction = ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue);
                    break;
            }
        }

        private float ConvertControllerValueToPercentage(int controllerValue)
        {
            // controller value ranges between 0 - 127
            throw new NotImplementedException();
        }


        public static List<MidiWithIndex> GetDevices()
        {
            List<MidiWithIndex> devices = new List<MidiWithIndex>();
            
            for (int index = 0; index < MidiIn.NumberOfDevices; index++)
            {
                MidiInCapabilities x = MidiIn.DeviceInfo(index);
                devices.Add(new MidiWithIndex(x, index));
            }

            return devices;
        }
    }

    public class MidiWithIndex
    {
        public MidiInCapabilities Device { get; }
        public int Index { get; }

        public MidiWithIndex(MidiInCapabilities device, int index)
        {
            Device = device;
            Index = index;
        }
    }
}
