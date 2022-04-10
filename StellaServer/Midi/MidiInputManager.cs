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
        private const float _RGB_CONVERSION_RATIO = 100.0f / 127.0f / 100.0f;
        private const float _BRIGHTNESS_CONVERSION_RATIO = 2.0f / 127.0f;
        private const float _SPEED_RATIO = 1.5f;

        private readonly int _deviceIndex;
        private MidiIn _midiIn;
        private StellaServerLib.StellaServer _stellaServer;

        [Reactive] public float RedCorrection { get; set; }
        [Reactive] public float GreenCorrection { get; set; }
        [Reactive] public float BlueCorrection { get; set; }



        public MidiInputManager(int deviceIndex)
        {
            _deviceIndex = deviceIndex;
        }

        public void Start(StellaServerLib.StellaServer stellaServer)
        {
            _stellaServer = stellaServer;
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
                case 2: // BRIGHTNESS
                {
                    _stellaServer.Animator.StoryboardTransformationController.SetBrightnessCorrection(ConvertToBrightnessScale(controlChangeEvent.ControllerValue));
                        break;
                }

                case 9: // SPEED
                {
                    int value = controlChangeEvent.ControllerValue;
                    if (value == 127)
                    {
                        value = 1000;
                    }


                    _stellaServer.Animator.StoryboardTransformationController.SetTimeUnitsPerFrame((int)(value));
                    break;
                }

                case 6: // RED
                {
                    var master = _stellaServer.Animator?.StoryboardTransformationController.Settings.MasterSettings;
                    if (master == null)
                    {
                        return;
                    }


                    _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(new float[3]
                    {
                        ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue),
                        master.RgbFadeCorrection[1],
                        master.RgbFadeCorrection[2],
                    });
                }
                    break;
                case 7: // GREEN
                {
                    var master = _stellaServer.Animator?.StoryboardTransformationController.Settings.MasterSettings;
                    if (master == null)
                    {
                        return;
                    }

                    _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(new float[3]
                    {
                        master.RgbFadeCorrection[0],
                        ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue),
                        master.RgbFadeCorrection[2],
                    });
                }
                    break;
                case 8: // BLUE
                {
                    var master = _stellaServer.Animator?.StoryboardTransformationController.Settings.MasterSettings;
                    if (master == null)
                    {
                        return;
                    }
                    
                    _stellaServer.Animator.StoryboardTransformationController.SetRgbFadeCorrection(new float[3]
                    {
                        master.RgbFadeCorrection[0],
                        master.RgbFadeCorrection[1],
                        ConvertControllerValueToPercentage(controlChangeEvent.ControllerValue),
                    });
                }
                    break;
            }
        }

        private float ConvertControllerValueToPercentage(int controllerValue)
        {
            // controller value ranges between 0 - 127
            // the rgb correction should be in the range 0 - 100
            // and then converted to 0 - 1 for the transformationsettings
            return controllerValue * _RGB_CONVERSION_RATIO;
        }

        private float ConvertToBrightnessScale(int controllerValue)
        {
            // controller value ranges between 0 - 127
            // the rgb correction should be in the range -100 - 100
            // and then converted to -1 - 1 for the transformationsettings

            return -1 + controllerValue * _BRIGHTNESS_CONVERSION_RATIO;
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
