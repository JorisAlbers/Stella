using System;
using System.Reactive.Linq;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServer.Animation.Details;
using StellaServer.Log;
using StellaServer.Status;
using StellaServer.Transformations;
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

        [Reactive] public TransformationViewModel TransformationViewModel { get; set; }
        [Reactive] public ReactiveObject SelectedViewModel { get; set; }

        public MainControlPanelViewModel(StellaServerLib.StellaServer stellaServer,
            StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator,
            BitmapRepository bitmapRepository, LogViewModel logViewModel)
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
                        
                        if (onNext.Animation is Storyboard storyboard)
                        {
                            SelectedViewModel = new StoryboardDetailsControlViewModel(storyboard, bitmapRepository);
                            return;
                        }

                        if(onNext.Animation is PlayList playList)
                        {
                            SelectedViewModel = new PlaylistDetailsContolViewModel(playList, bitmapRepository);
                            return;
                        }

                        throw new NotImplementedException("Unknown animation type");
                        
                    });

            StatusViewModel = new StatusViewModel(stellaServer, 3, logViewModel); //TODO insert number of clients
            
        }

        private void AnimationsPanelViewModelOnStartAnimationRequested(object sender, IAnimation e)
        {
            Console.WriteLine($"Starting {e.Name}");
            _stellaServer.StartAnimation(e);
            StatusViewModel.AnimationStarted(e);
            if (TransformationViewModel == null)
            {
                TransformationViewModel = new TransformationViewModel(_stellaServer);
            }
        }
    }
}