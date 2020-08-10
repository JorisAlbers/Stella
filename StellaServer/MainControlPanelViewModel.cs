using System;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServer.Animation.Details;
using StellaServer.Status;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaServer
{
    public class MainControlPanelViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public CurrentlyPlayingViewModel CurrentlyPlayingViewModel { get; set; }
        [Reactive] public AnimationsPanelViewModel AnimationsPanelViewModel { get; set; }

        [Reactive] public StatusViewModel StatusViewModel { get; set; }
        [Reactive] public ReactiveObject SelectedViewModel { get; set; }

        public MainControlPanelViewModel(StellaServerLib.StellaServer stellaServer, StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator, BitmapRepository bitmapRepository)
        {
            _stellaServer = stellaServer;
            AnimationsPanelViewModel = new AnimationsPanelViewModel(storyboardRepository,bitmapStoryboardCreator,bitmapRepository);
            AnimationsPanelViewModel.StartAnimationRequested += AnimationsPanelViewModelOnStartAnimationRequested;
            this.WhenAnyValue(x => x.AnimationsPanelViewModel.SelectedAnimation)
                .Subscribe(onNext =>
                    {
                        if (onNext == null)
                        {
                            SelectedViewModel = null;
                            return;
                        }

                        var vm = new StoryboardDetailsControlViewModel(onNext.Storyboard, bitmapRepository);
                        SelectedViewModel = vm;
                    });



            StatusViewModel = new StatusViewModel(stellaServer, 3); //TODO insert number of clients
            
            
        }

        private void AnimationsPanelViewModelOnStartAnimationRequested(object sender, IAnimation e)
        {
            Console.WriteLine($"Starting {e.Name}");
            _stellaServer.StartAnimation(e);
            StatusViewModel.AnimationStarted(e);
        }
    }
}