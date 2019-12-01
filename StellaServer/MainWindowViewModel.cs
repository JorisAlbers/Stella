using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using StellaServer.ServerConfiguration;

namespace StellaServer
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ServerConfigurationViewModel SeverConfigurationViewModel { get; }

        public MainWindowViewModel()
        {
            SeverConfigurationViewModel = new ServerConfigurationViewModel();
            SeverConfigurationViewModel.SaveRequested += SeverConfigurationViewModel_OnSaveRequested;
        }

        private void SeverConfigurationViewModel_OnSaveRequested(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
