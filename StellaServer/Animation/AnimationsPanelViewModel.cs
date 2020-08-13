using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
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
        private readonly StoryboardRepository _storyboardRepository;
        private readonly BitmapStoryboardCreator _bitmapStoryboardCreator;
        private readonly BitmapRepository _bitmapRepository;

        [Reactive] public List<AnimationPanelItemViewModel> AnimationViewModels { get; set; }

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
            

            AnimationViewModels = animations.Select(x=> new AnimationPanelItemViewModel(x)).Select(vm =>
                {
                    vm.StartCommand.Subscribe(onNext => { StartAnimationRequested?.Invoke(this, vm.Animation); });
                    return vm;
                }).ToList();
            
            /*// TODO why is this not working?
            this.WhenAnyValue(x => x.SelectedAnimation)
                .Where(x=> x!=null)
                .Select(x => new StoryboardDetailsControlViewModel(SelectedAnimation.Animation))
                .ToProperty(this, x=> x.StoryboardDetails);*/

        }
    }
}