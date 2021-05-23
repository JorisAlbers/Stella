using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VideoMapping
{
    public class SettingsSerializer
    {
        private readonly FileInfo _settingsFile;

        public SettingsSerializer(FileInfo settingsFile)
        {
            _settingsFile = settingsFile;
        }

        public Settings Load()
        {
            using StreamReader reader = _settingsFile.OpenText();
            string data = reader.ReadToEnd();
            return JsonConvert.DeserializeObject<Settings>(data);
        }

        public void Save(Settings settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            using StreamWriter writer = new StreamWriter(_settingsFile.OpenWrite());
            writer.Write(json);
        }
    }
}
