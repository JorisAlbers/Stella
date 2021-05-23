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


        public MainWindowViewModel()
        {
            
        }

    }
}