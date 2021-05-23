using System.Reactive;
using ReactiveUI;

namespace VideoMapping
{
    public class MainWindowViewModel : ReactiveObject
    {
        public int PixelsPerRow { get; set; } = 480;
        public int Rows { get; set; } = 6;

        public string StoryboardFolder { get; set; }
        public string OutputFolder { get; set; }
        public string InputFolder { get; set; }

        public ReactiveCommand<Unit, Settings> Start { get; }


        public MainWindowViewModel()
        {
            Start = ReactiveCommand.Create<Unit, Settings>((_) => new Settings(PixelsPerRow, Rows, StoryboardFolder, OutputFolder, InputFolder));
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