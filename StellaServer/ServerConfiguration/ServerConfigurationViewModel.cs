using System;
using System.Windows.Input;

namespace StellaServer.ServerConfiguration
{
    public class ServerConfigurationViewModel
    {
        private ICommand _saveCommand;

        public event EventHandler SaveRequested;

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string MappingFilePath { get; set; }

        public string StoryboardFolderPath { get; set; }

        public string BitmapFolderPath { get; set; }
        
        public ICommand SaveCommand
        {
            get { return _saveCommand ??= new RelayCommand((param) => { OnSaveRequested(); }); }
        }

        public ServerConfigurationViewModel()
        {
        }

        private void OnSaveRequested()
        {
            SaveRequested?.Invoke(this, new EventArgs());
        }
    }
}