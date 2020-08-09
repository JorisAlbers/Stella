using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib.Animation;

namespace StellaServer.Animation
{
    public class AnimationPanelItemViewModel : ReactiveObject
    {
        public Storyboard Storyboard { get; }

        public string Name { get; set; }

        public ReactiveCommand<Unit, Unit> StartCommand { get; } = ReactiveCommand.Create(() => { });
            
        public AnimationPanelItemViewModel(Storyboard storyboard)
        {
            Storyboard = storyboard;
            Name = storyboard.Name;
        }
    }
}