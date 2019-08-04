using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaServerLib.Animation;

namespace StellaTestSuite.Server
{
    public class ServerControlPanelViewModel : INotifyPropertyChanged
    {
        public List<Storyboard> Storyboards { get; private set; }

        public Storyboard SelectedStoryboard { get; set; }

        public ServerControlPanelViewModel(List<Storyboard> storyboards)
        {
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
