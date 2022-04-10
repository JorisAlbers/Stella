using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using ReactiveUI;
using NAudio.Midi;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Midi
{
    public class MidiInputManager : ReactiveObject
    {
        private const float _CONVERSION_RATIO = 100.0f / 127.0f;
        private readonly int _deviceIndex;
        private MidiIn _midiIn;

        [Reactive] public float RedCorrection { get; set; }
        [Reactive] public float GreenCorrection { get; set; }
        [Reactive] public float BlueCorrection { get; set; }



        public MidiInputManager(int deviceIndex)
        {
            _deviceIndex = deviceIndex;
        }

        public void Start()
        {
            _midiIn = new MidiIn(_deviceIndex);
            _midiIn.MessageReceived += MidiInOnMessageReceived;
            _midiIn.ErrorReceived += MidiInOnErrorReceived;
            _midiIn.Start();
        }

        private void MidiInOnErrorReceived(object? sender, MidiInMessageEventArgs e)
        {
            Console.Out.WriteLine(e.RawMessage);
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
                case 6: // RED
                    RedCorrection = ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue);
                    break;
                case 7: // GREEN
                    GreenCorrection = ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue);
                    break;
                case 8: // BLUE
                    BlueCorrection = ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue);
                    break;
            }
        }

        private float ConvertControllerValueToPercentage(int controllerValue)
        {
            // controller value ranges between 0 - 127
            // the rgb correction should be in the range 0 - 100
            return controllerValue * _CONVERSION_RATIO;
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
