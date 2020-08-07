using System.Collections.Generic;
using System.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;

namespace StellaServer.Animation
{
    public class AnimationsPanelViewModel : ReactiveObject
    {
        private readonly StoryboardRepository _storyboardRepository;
        private readonly BitmapStoryboardCreator _bitmapStoryboardCreator;
        private readonly BitmapRepository _bitmapRepository;

        [Reactive] public List<AnimationPanelItemViewModel> StoryboardViewModels { get; set; }


        public AnimationsPanelViewModel(StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _storyboardRepository = storyboardRepository;
            _bitmapStoryboardCreator = bitmapStoryboardCreator;
            _bitmapRepository = bitmapRepository;

            StoryboardViewModels = storyboardRepository.LoadStoryboards().Select(x=> new AnimationPanelItemViewModel(x)).ToList();
        }
    }
}