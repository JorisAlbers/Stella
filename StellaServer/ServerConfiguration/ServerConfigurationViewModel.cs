using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Win32;

namespace StellaServer.ServerConfiguration
{
    public class ServerConfigurationViewModel : INotifyPropertyChanged
    {
        private ICommand _saveCommand;
        private ICommand _browseToMappingFileCommand;
        private ICommand _browseToStoryboardFolderPathCommand;
        private ICommand _browseToBitmapFolderPathCommand;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public ICommand BrowseToMappingFileCommand
        {
            get
            {
                return _browseToMappingFileCommand ??= new RelayCommand((param) =>
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    if (fileDialog.ShowDialog().GetValueOrDefault())
                    {
                        MappingFilePath = fileDialog.FileName;
                    }
                });
            }
        }
        public ICommand BrowseToStoryboardFolderPathCommand
        {
            get
            {
                return _browseToStoryboardFolderPathCommand ??= new RelayCommand((param) =>
                {
                    // Create a "Save As" dialog for selecting a directory (HACK)
                    var dialog = new Microsoft.Win32.SaveFileDialog();
                    dialog.InitialDirectory = StoryboardFolderPath; // Use current value for initial dir
                    dialog.Title = "Select a Directory"; // instead of default "Save As"
                    dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
                    dialog.FileName = "select"; // Filename will then be "select.this.directory"
                    if (dialog.ShowDialog() == true)
                    {
                        string path = dialog.FileName;
                        // Remove fake filename from resulting path
                        path = path.Replace("\\select.this.directory", "");
                        path = path.Replace(".this.directory", "");
                        // If user has changed the filename, create the new directory
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        // Our final value is in path
                        StoryboardFolderPath = path;
                    }
                });
            }
        }

        public ICommand BrowseToBitmapFolderPathCommand
        {
            get
            {
                return _browseToBitmapFolderPathCommand ??= new RelayCommand((param) =>
                {
                    // Create a "Save As" dialog for selecting a directory (HACK)
                    var dialog = new Microsoft.Win32.SaveFileDialog();
                    dialog.InitialDirectory = BitmapFolderPath; // Use current value for initial dir
                    dialog.Title = "Select a Directory"; // instead of default "Save As"
                    dialog.Filter = "Directory|*.this.directory"; // Prevents displaying files
                    dialog.FileName = "select"; // Filename will then be "select.this.directory"
                    if (dialog.ShowDialog() == true)
                    {
                        string path = dialog.FileName;
                        // Remove fake filename from resulting path
                        path = path.Replace("\\select.this.directory", "");
                        path = path.Replace(".this.directory", "");
                        // If user has changed the filename, create the new directory
                        if (!System.IO.Directory.Exists(path))
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        // Our final value is in path
                        BitmapFolderPath = path;
                    }
                });
            }
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