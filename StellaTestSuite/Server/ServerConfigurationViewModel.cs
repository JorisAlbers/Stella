using System.ComponentModel;

namespace StellaTestSuite.Server
{
    public class ServerConfigurationViewModel : INotifyPropertyChanged
    {
        public string StoryboardDirectory { get; set; }
        public string BitmapDirectory { get; set; }
        public string ConfigurationFile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Apply()
        {
            throw new System.NotImplementedException();
        }
    }
}