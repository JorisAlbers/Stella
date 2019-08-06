using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace StellaVisualizer.Server
{
    public class ServerConfigurationViewModel : INotifyPropertyChanged
    {
        private const string CONFIG_FILE = "serverConfigFile.csv";

        public string StoryboardDirectory { get; set; }
        public string BitmapDirectory { get; set; }
        public string ConfigurationFile { get; set; }
        public string ApiServerIpAddress { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler ApplyRequested;

        public ServerConfigurationViewModel()
        {
            // Try to load prev. saved
            if (File.Exists(CONFIG_FILE))
            {
                string[] lines = File.ReadAllLines(CONFIG_FILE);
                foreach (string line in lines)
                {
                    string[] split = line.Split(';');
                    switch (split[0])
                    {
                        case "StoryboardDirectory":
                            StoryboardDirectory = split[1];
                            break;
                        case "BitmapDirectory":
                            BitmapDirectory = split[1];
                            break;
                        case "ConfigurationFile":
                            ConfigurationFile = split[1];
                            break;
                        case "ApiServerIpAddress":
                            ApiServerIpAddress = split[1];
                            break;
                    }
                }
            }

        }



        public void Apply()
        {
            // Save to config file
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"StoryboardDirectory;{StoryboardDirectory}");
            sb.AppendLine($"BitmapDirectory;{BitmapDirectory}");
            sb.AppendLine($"ConfigurationFile;{ConfigurationFile}");
            sb.AppendLine($"ApiServerIpAddress;{ApiServerIpAddress}");
            File.WriteAllText(CONFIG_FILE,sb.ToString());

            var eventHandler = ApplyRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,new EventArgs());
            }
        }

    }
}