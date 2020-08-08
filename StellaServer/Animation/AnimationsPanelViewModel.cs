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

        [Reactive] public StoryboardDetailsControlViewModel StoryboardDetails { get; set; }


        public AnimationsPanelViewModel(StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _storyboardRepository = storyboardRepository;
            _bitmapStoryboardCreator = bitmapStoryboardCreator;
            _bitmapRepository = bitmapRepository;

            StoryboardViewModels = storyboardRepository.LoadStoryboards().Select(x=> new AnimationPanelItemViewModel(x)).ToList();

            /*// TODO why is this not working?
            this.WhenAnyValue(x => x.SelectedAnimation)
                .Where(x=> x!=null)
                .Select(x => new StoryboardDetailsControlViewModel(SelectedAnimation.Storyboard))
                .ToProperty(this, x=> x.StoryboardDetails);*/

            // TODO how to implement this properly with reactiveUi?
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectedAnimation))
            {
                if (SelectedAnimation != null)
                {
                    StoryboardDetails = new StoryboardDetailsControlViewModel(SelectedAnimation.Storyboard);
                }
            }
        }
    }
}