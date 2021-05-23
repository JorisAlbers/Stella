namespace VideoMapping
{
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