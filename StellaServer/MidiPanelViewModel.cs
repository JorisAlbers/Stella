using System;
using System.Collections.Generic;
using System.Reactive;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Midi;
using StellaServerLib.Animation;

namespace StellaServer
{
    public class MidiPanelViewModel : ReactiveObject
    {
        private readonly int _controllerStartIndex;
        private readonly MidiInputManager _midiInputManager;
        public int Rows { get; }
        public int Columns { get; }


        public MidiPadButtonViewModel[] Pads { get; }

        public ReactiveCommand<IAnimation, IAnimation> StartAnimation { get; }


        public MidiPanelViewModel(int rows, int columns, int controllerStartIndex, MidiInputManager midiInputManager)
        {
            _controllerStartIndex = controllerStartIndex;
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
                var viewmodel = Pads[x.ControllerIndex - controllerStartIndex];
                viewmodel.PadPressed(x);


                if (!viewmodel.KeyDown && viewmodel.AnimationName != null)
                {
                    StartAnimation.Execute(viewmodel.Animation).Subscribe().Dispose();
                }
            });

            StartAnimation = ReactiveCommand.Create<IAnimation, IAnimation>(unit =>
            {
                return unit;
            });


        }

        public void SetAnimationToPad(IAnimation animation, int padIndex)
        {
            Pads[padIndex].SetAnimation(animation);
        }
    }

    public class MidiPadButtonViewModel : ReactiveObject
    {

        public int Controller { get; }

        [Reactive] public bool KeyDown { get; private set; }

        [Reactive] public string AnimationName { get;private set; }
        
        [Reactive] public IAnimation Animation { get; private set; }

        public MidiPadButtonViewModel(int controller)
        {
            Controller = controller;
        }

        public void PadPressed(PadPressedEvent padPressedEvent)
        {
            KeyDown = padPressedEvent.KeyDown;
        }

        public void SetAnimation(IAnimation animation)
        {
            Animation = animation;
            AnimationName = Animation.Name;
        }
    }
}