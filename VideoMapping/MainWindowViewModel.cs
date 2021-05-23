using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace VideoMapping
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive]
        public int PixelsPerRow { get; set; } = 480;
        [Reactive]
        public int Rows { get; set; } = 6;
        [Reactive]
        public string StoryboardFolder { get; set; }
        [Reactive]
        public string OutputFolder { get; set; }
        [Reactive]
        public string InputFolder { get; set; }

        public ReactiveCommand<Unit, Settings> Start { get; }


        public MainWindowViewModel()
        {
            SettingsSerializer settingsSerializer = new SettingsSerializer(new FileInfo("settings.json"));
            try
            {
                Settings settings = settingsSerializer.Load();
                PixelsPerRow = settings.PixelsPerRow;
                Rows = settings.Rows;
                StoryboardFolder = settings.StoryboardFolder;
                OutputFolder = settings.OutputFolder;
                InputFolder = settings.InputFolder;
            }
            catch { }
            
            Start = ReactiveCommand.Create<Unit, Settings>((_) => new Settings(PixelsPerRow, Rows, StoryboardFolder, OutputFolder, InputFolder));
            Start.Subscribe((x) =>
            {
                settingsSerializer.Save(x);
                VideosConverter converter = new VideosConverter(new DirectoryInfo(x.InputFolder), new DirectoryInfo(x.OutputFolder), x.Rows, x.PixelsPerRow);
                converter.Start();
            });
            Start.ThrownExceptions.Subscribe(x =>
            {
                MessageBox.Show(x.Message, "error");
            });

        }

    }
}