using System;
using System.Collections.Generic;
using System.ComponentModel;
using PropertyChanged;
using StellaServerLib;
using StellaServerLib.Animation;
using StellaServerLib.Network;

namespace StellaVisualizer.Server
{
    public class ServerControlPanelViewModel : INotifyPropertyChanged
    {
        private readonly StellaServer _stellaServer;
        public List<Storyboard> Storyboards { get; private set; }

        public Storyboard SelectedStoryboard { get; set; }

        public int MasterWaitMs { get; set; }

        public float MasterRedCorrection { get; set; }
        public float MasterGreenCorrection { get; set; }
        public float MasterBlueCorrection { get; set; }
        
        public ServerControlPanelViewModel(StellaServerLib.StellaServer stellaServer, List<Storyboard> storyboards)
        {
            _stellaServer = stellaServer;
            Storyboards = storyboards;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedStoryboard):
                    StartStoryboard(SelectedStoryboard);
                    break;
                case nameof(MasterWaitMs):
                    _stellaServer.Animator.AnimationTransformations.SetFrameWaitMs(MasterWaitMs);
                    break;
                case nameof(MasterRedCorrection):
                case nameof(MasterBlueCorrection):
                case nameof(MasterGreenCorrection):
                    _stellaServer.Animator.AnimationTransformations.SetRgbFadeCorrection(new float[]{MasterRedCorrection, MasterGreenCorrection, MasterBlueCorrection });
                    break;
            }
        }

        private void StartStoryboard(Storyboard selectedStoryboard)
        {
            if (selectedStoryboard == null)
            {
                return;
            }

            var eventHandler = StartStoryboardRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,selectedStoryboard);
            }
        }

        public event EventHandler<Storyboard> StartStoryboardRequested;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
