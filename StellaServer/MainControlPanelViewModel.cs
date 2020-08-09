using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaServer
{
    public class MainControlPanelViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public CurrentlyPlayingViewModel CurrentlyPlayingViewModel { get; set; }
        [Reactive] public AnimationsPanelViewModel AnimationsPanelViewModel { get; set; }
        [Reactive] public ReactiveObject SelectedViewModel { get; set; }

        public MainControlPanelViewModel(StellaServerLib.StellaServer stellaServer, StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _stellaServer = stellaServer;
            AnimationsPanelViewModel = new AnimationsPanelViewModel(storyboardRepository,bitmapStoryboardCreator,bitmapRepository);
            AnimationsPanelViewModel.StartAnimationRequested += AnimationsPanelViewModelOnStartAnimationRequested;
            
            SelectedViewModel = AnimationsPanelViewModel;
        }

        private void AnimationsPanelViewModelOnStartAnimationRequested(object sender, IAnimation e)
        {
            Console.WriteLine($"Starting {e.Name}");
            _stellaServer.StartAnimation(e);
        }
    }
}