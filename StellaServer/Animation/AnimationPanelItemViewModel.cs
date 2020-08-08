using System;
using System.Reactive;
using ReactiveUI;
using StellaServerLib.Animation;

namespace StellaServer.Animation
{
    public class AnimationPanelItemViewModel : ReactiveObject
    {
        public Storyboard Storyboard { get; }

        public string Name { get; set; }

        public ReactiveCommand<Unit, Unit> StartCommand { get; }

        public AnimationPanelItemViewModel(Storyboard storyboard)
        {
            Storyboard = storyboard;
            Name = storyboard.Name;

            StartCommand = ReactiveCommand.Create(() =>
            {
                // TODO implement start
               Console.WriteLine("Lets start!");
            });

        }
    }
}