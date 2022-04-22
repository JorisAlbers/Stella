using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServer.Animation.Creation;
using StellaServer.Animation.Details;
using StellaServer.Log;
using StellaServer.Midi;
using StellaServer.Status;
using StellaServer.Transformations;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaServer
{
    public class MainControlPanelViewModel : ReactiveObject
    {
        private readonly StellaServerLib.StellaServer _stellaServer;
        [Reactive] public AnimationCreationViewModel AnimationCreationViewModel { get; set; }
        [Reactive] public AnimationsPanelViewModel AnimationsPanelViewModel { get; set; }
        [Reactive] public StatusViewModel StatusViewModel { get; set; }
        [Reactive] public TransformationViewModel TransformationViewModel { get; set; }
        [Reactive] public NavigationViewModel NavigationViewModel { get; set; }

        [Reactive] public MidiPanelViewModel MidiPanelViewModel { get; set; }

        [Reactive] public ReactiveObject SelectedViewModel { get; set; }

        public MainControlPanelViewModel(StellaServerLib.StellaServer stellaServer,
            StoryboardRepository storyboardRepository, BitmapStoryboardCreator bitmapStoryboardCreator,
            BitmapRepository bitmapRepository, BitmapThumbnailRepository thumbnailRepository, LogViewModel logViewModel,
            MidiInputManager midiInputManager)
        {
            _stellaServer = stellaServer;
            AnimationsPanelViewModel = new AnimationsPanelViewModel(storyboardRepository,bitmapStoryboardCreator,bitmapRepository);
            AnimationsPanelViewModel.StartAnimationRequested += StartAnimation;
            AnimationsPanelViewModel.SendToPadRequested += AnimationsPanelViewModel_OnSendToPadRequested;

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
                            var viewmodel = new StoryboardDetailsControlViewModel(storyboard, bitmapRepository);
                            viewmodel.Back.Subscribe(next => { SelectedViewModel = NavigationViewModel; });
                            SelectedViewModel = viewmodel;
                            return;
                        }

                        if(onNext.Animation is PlayList playList)
                        {
                            var viewmodel = new PlaylistDetailsContolViewModel(playList, bitmapRepository);
                            viewmodel.Back.Subscribe(next => { SelectedViewModel = NavigationViewModel; });
                            SelectedViewModel = viewmodel;
                            return;
                        }

                        throw new NotImplementedException("Unknown animation type");
                        
                    });

            StatusViewModel = new StatusViewModel(stellaServer, 3, logViewModel); //TODO insert number of clients
            AnimationCreationViewModel = new AnimationCreationViewModel(bitmapRepository,bitmapStoryboardCreator,thumbnailRepository, 6, 24); // Todo insert number of rows, number of tubes
            AnimationCreationViewModel.Save.Subscribe(onNext => AnimationsPanelViewModel.AddItem(onNext));
            AnimationCreationViewModel.Start.Subscribe(onNext => StartAnimation(null, onNext));
            AnimationCreationViewModel.Back.Subscribe(onNext => SelectedViewModel = NavigationViewModel);

            if (midiInputManager != null)
            {
                MidiPanelViewModel = new MidiPanelViewModel(4, 4, 10, midiInputManager); //TODO configurable midi buttons
                MidiPanelViewModel.StartAnimation.Subscribe(x =>
                {
                    StartAnimation(null, x);
                });
                midiInputManager.Stop.Subscribe(x =>
                {
                    StartAnimation(null, FindStopAnimation());
                });
            }
            
            NavigationViewModel = new NavigationViewModel();
            NavigationViewModel.NavigateToCreateAnimation.Subscribe(onNext => SelectedViewModel = AnimationCreationViewModel);
            NavigationViewModel.NavigateToMidiPanel.Subscribe(onNext => SelectedViewModel = MidiPanelViewModel);
            SelectedViewModel = NavigationViewModel;
        }

        private void AnimationsPanelViewModel_OnSendToPadRequested(object? sender, SendToPadEventArgs e)
        {
            MidiPanelViewModel.SetAnimationToPad(e.Animation, e.Pad);
        }

        private void StartAnimation(object sender, IAnimation e)
        {
            Console.WriteLine($"Starting {e.Name}");
            _stellaServer.StartAnimation(e);
            StatusViewModel.AnimationStarted(e);
            if (TransformationViewModel == null)
            {
                TransformationViewModel = new TransformationViewModel(_stellaServer, FindStopAnimation());
                TransformationViewModel.Stop.Subscribe(x =>
                {
                    StartAnimation(null, FindStopAnimation());
                });
            }
        }

        

        private IAnimation FindStopAnimation()
        {
            return AnimationsPanelViewModel.Items.FirstOrDefault(x => x.Name == "Clear")?.Animation;
        }
    }
}