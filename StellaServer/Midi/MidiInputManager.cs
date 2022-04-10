using System;
using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using NAudio.Midi;

namespace StellaServer.Midi
{
    public class MidiInputManager : ReactiveObject
    {
        private static List<MidiWithIndex> GetDevices()
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
