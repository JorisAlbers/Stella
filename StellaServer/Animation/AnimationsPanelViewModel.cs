using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation.Details;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation
{
    public class AnimationsPanelViewModel : ReactiveObject
    {
        private readonly SourceList<AnimationPanelItemViewModel> _animationViewModels = new SourceList<AnimationPanelItemViewModel>();
        public ReadOnlyObservableCollection<AnimationPanelItemViewModel> Items { get; }

        private readonly StoryboardRepository _storyboardRepository;
        private readonly BitmapStoryboardCreator _bitmapStoryboardCreator;
        private readonly BitmapRepository _bitmapRepository;

        [Reactive] public AnimationPanelItemViewModel SelectedAnimation { get; set; }

        
        //TODO use observable
        public event EventHandler<IAnimation> StartAnimationRequested;

        public AnimationsPanelViewModel(StoryboardRepository storyboardRepository,
            BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _storyboardRepository = storyboardRepository;
            _bitmapStoryboardCreator = bitmapStoryboardCreator;
            _bitmapRepository = bitmapRepository;
            
            // Load storyboards animations
            List<Storyboard> storyboards = storyboardRepository.LoadStoryboards();
            // Load bitmap animations
            storyboards.AddRange(_bitmapStoryboardCreator.Create());
            // Create play lists
            List<IAnimation> animations = storyboards.Cast<IAnimation>().ToList();
            animations.Add(PlaylistCreator.Create("All combined", storyboards, 120));
            animations.AddRange(PlaylistCreator.CreateFromCategory(storyboards, 120));

            var list = animations.Select(x => new AnimationPanelItemViewModel(x)).Select(vm =>
            {
                vm.StartCommand.Subscribe(onNext => { StartAnimationRequested?.Invoke(this, vm.Animation); });
                return vm;
            }).ToList();
            _animationViewModels.AddRange(list);

            _animationViewModels.Connect().Bind(out var animationPanelItemViewModels).Subscribe();
            Items = animationPanelItemViewModels;

           

            /*// TODO why is this not working?
            this.WhenAnyValue(x => x.SelectedAnimation)
                .Where(x=> x!=null)
                .Select(x => new StoryboardDetailsControlViewModel(SelectedAnimation.Animation))
                .ToProperty(this, x=> x.StoryboardDetails);*/
        }

        public void AddItem(IAnimation animation)
        {
            _animationViewModels.Add(new AnimationPanelItemViewModel(animation));
        }
    }
}