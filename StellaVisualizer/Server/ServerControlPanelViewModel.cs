using System;
using System.Collections.Generic;
using System.ComponentModel;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaVisualizer.Server
{
    public class ServerControlPanelViewModel : INotifyPropertyChanged
    {
        private readonly StellaServer _stellaServer;
        public List<IAnimation> Animations { get; private set; }

        public IAnimation SelectedAnimation { get; set; }

        public int MasterWaitMs { get; set; }

        public float MasterRedCorrection { get; set; }
        public float MasterGreenCorrection { get; set; }
        public float MasterBlueCorrection { get; set; }
        
        public ServerControlPanelViewModel(StellaServerLib.StellaServer stellaServer, List<IAnimation> animations)
        {
            _stellaServer = stellaServer;
            Animations = animations;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedAnimation):
                    StartStoryboard(SelectedAnimation);
                    break;
                case nameof(MasterWaitMs):
                    _stellaServer.Animator.TransformationController.SetTimeUnitsPerFrame(MasterWaitMs);
                    break;
                case nameof(MasterRedCorrection):
                case nameof(MasterBlueCorrection):
                case nameof(MasterGreenCorrection):
                    _stellaServer.Animator.TransformationController.SetRgbFadeCorrection(new float[]{MasterRedCorrection, MasterGreenCorrection, MasterBlueCorrection });
                    break;
            }
        }

        private void StartStoryboard(IAnimation selectedAnimation)
        {
            if (selectedAnimation == null)
            {
                return;
            }

            var eventHandler = StartAnimationRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,selectedAnimation);
            }
        }

        public event EventHandler<IAnimation> StartAnimationRequested;

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
