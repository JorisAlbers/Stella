using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServerLib;

namespace StellaServer
{
    public class MainControlPanelViewModel : ReactiveObject
    {
        
        [Reactive] public CurrentlyPlayingViewModel CurrentlyPlayingViewModel { get; set; }
        [Reactive] public AnimationsPanelViewModel AnimationsPanelViewModel { get; set; }
        [Reactive] public ReactiveObject SelectedViewModel { get; set; }

        public MainControlPanelViewModel(StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            AnimationsPanelViewModel = new AnimationsPanelViewModel(storyboardRepository,bitmapStoryboardCreator,bitmapRepository);

            //TODO select currently playing by default
            SelectedViewModel = AnimationsPanelViewModel;
        }
    }
}