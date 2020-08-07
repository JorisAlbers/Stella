using System;
using System.Reactive;
using ReactiveUI;
using StellaServerLib.Animation;

namespace StellaServer.Animation
{
    public class AnimationPanelItemViewModel : ReactiveObject
    {
        private readonly Storyboard _storyboard;

        public string Name { get; set; }

        // TODO add animation settings

        public ReactiveCommand<Unit, Unit> StartCommand { get; }

        public AnimationPanelItemViewModel(Storyboard storyboard)
        {
            _storyboard = storyboard;
            Name = storyboard.Name;

            StartCommand = ReactiveCommand.Create(() =>
            {
                // TODO implement start
               Console.Beep();
            });

        }
    }
}