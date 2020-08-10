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

        [Reactive] public List<AnimationPanelItemViewModel> StoryboardViewModels { get; set; }

        [Reactive] public AnimationPanelItemViewModel SelectedAnimation { get; set; }

        
        //TODO use observable
        public event EventHandler<IAnimation> StartAnimationRequested;

        public AnimationsPanelViewModel(StoryboardRepository storyboardRepository,
            BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _storyboardRepository = storyboardRepository;
            _bitmapStoryboardCreator = bitmapStoryboardCreator;
            _bitmapRepository = bitmapRepository;

            StoryboardViewModels = storyboardRepository
                .LoadStoryboards().Select(x => new AnimationPanelItemViewModel(x))
                .Union(_bitmapStoryboardCreator.Create().Select(x => new AnimationPanelItemViewModel(x)))
                .Select(vm =>
                {
                    vm.StartCommand.Subscribe(onNext => { StartAnimationRequested?.Invoke(this, vm.Storyboard); });
                    return vm;
                }).ToList();


            /*// TODO why is this not working?
            this.WhenAnyValue(x => x.SelectedAnimation)
                .Where(x=> x!=null)
                .Select(x => new StoryboardDetailsControlViewModel(SelectedAnimation.Storyboard))
                .ToProperty(this, x=> x.StoryboardDetails);*/

        }
    }
}