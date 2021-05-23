using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
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
            });
        }

    }

    public class Settings
    {
        public int PixelsPerRow { get; }
        public int Rows { get; }
        public string StoryboardFolder { get; }
        public string OutputFolder { get; }
        public string InputFolder { get; }

        public Settings(int pixelsPerRow, int rows, string storyboardFolder, string outputFolder, string inputFolder)
        {
            PixelsPerRow = pixelsPerRow;
            Rows = rows;
            StoryboardFolder = storyboardFolder;
            OutputFolder = outputFolder;
            InputFolder = inputFolder;
        }
    }
}