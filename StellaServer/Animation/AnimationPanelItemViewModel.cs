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
        public IAnimation Animation { get; }

        public string Name { get; set; }

        public ReactiveCommand<Unit, Unit> StartCommand { get; } = ReactiveCommand.Create(() => { });

        public bool IsPlayList { get; set; }
            
        public AnimationPanelItemViewModel(IAnimation animation)
        {
            Animation = animation;
            Name = animation.Name;
            IsPlayList = animation is PlayList;
        }
    }
}