using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServer.Animation;
using StellaServer.Log;
using StellaServer.Setup;
using StellaServerLib;

namespace StellaServer
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly string UserSettingsFilePath =
            Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "StellaServer","Settings.xml");

        private UserSettings _userSettings;

        [Reactive] public ReactiveObject SelectedViewModel { get; set; }
        [Reactive] public LogViewModel LogViewModel { get; set; }
        
        public MainWindowViewModel()
        {
            LogViewModel = new LogViewModel();
            _userSettings = LoadUserSettings(UserSettingsFilePath);
            
            var setupViewModel = new SetupPanelViewModel(_userSettings?.ServerSetup);
            setupViewModel.ServerCreated += ServerCreated;
            SelectedViewModel = setupViewModel;
        }
        
        private void ServerCreated(object sender, ServerCreatedEventArgs args)
        {
           // Save user settings as there might be new settings
            _userSettings.ServerSetup = args.Settings;
            SaveUserSettings(UserSettingsFilePath, _userSettings);

            BitmapRepository bitmapRepository = new BitmapRepository(new FileSystem(), _userSettings.ServerSetup.BitmapFolder);
            BitmapStoryboardCreator bitmapStoryboardCreator = new BitmapStoryboardCreator(bitmapRepository,  360, 3, 2); // TODO get these magic values from the config
            StoryboardRepository storyboardRepository = new StoryboardRepository(_userSettings.ServerSetup.StoryboardFolder);

            SelectedViewModel = new MainControlPanelViewModel(args.StellaServer,storyboardRepository,bitmapStoryboardCreator,bitmapRepository, LogViewModel);
        }

        private UserSettings LoadUserSettings(string userSettingsFilePath)
        {
            FileInfo file = new FileInfo(userSettingsFilePath);
            
            if (file.Exists)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
                try
                {
                    using StreamReader reader = new StreamReader(file.FullName);
                    return (UserSettings) serializer.Deserialize(reader);
                }
                catch (Exception e)
                {
                    Console.Out.WriteLine("Failed to load user settings");
                    Console.Out.WriteLine(e.Message);
                }
            }

            return new UserSettings();
        }

        private void SaveUserSettings(string userSettingsFilePath, UserSettings userSettings)
        {
            FileInfo file = new FileInfo(userSettingsFilePath);

            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            try
            {
                using StreamWriter writer = new StreamWriter(userSettingsFilePath);
                serializer.Serialize(writer, userSettings);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("Failed to load user settings");
                Console.Out.WriteLine(e.Message);
            }
        }
    }
}
