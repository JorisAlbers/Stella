using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpYaml.Serialization;
using StellaLib.Serialization;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation.PlayLists;

namespace StellaServerLib.Serialization.Animation
{
    public class PlayListSerializer : ILoader<PlayList>
    {
        private readonly List<Storyboard> _storyboards;
        private readonly SerializerSettings _settings;
        private Serializer _serializer;

        public PlayListSerializer(List<Storyboard> storyboards)
        {
            _storyboards = storyboards;
            _settings = new SerializerSettings();
            _settings.EmitDefaultValues = true;
            _settings.RegisterAssembly(typeof(PlayListSettings).Assembly);
            _settings.RegisterTagMapping("Storyboards", typeof(PlayListItemSettings[]));
            _serializer = new Serializer(_settings);
        }

        public PlayList Load(StreamReader streamReader)
        {
            PlayListSettings settings = _serializer.Deserialize<PlayListSettings>(streamReader);

            // Validate playlist properties
            if (String.IsNullOrWhiteSpace(settings.Name))
            {
                throw new FormatException($"Failed to load the playlist. The name must be set.");
            }

            List<PlayListItem> items = new List<PlayListItem>();
          
            // Retrieve the storyboards by name
            for (var index = 0; index < settings.StoryboardSettings.Length; index++)
            {
                PlayListItemSettings setting = settings.StoryboardSettings[index];
                Storyboard storyboard = _storyboards.FirstOrDefault(x => x.Name == setting.Name);
                if (storyboard == null)
                {
                    throw new InvalidDataException($"The storyboard {setting.Name} of item {index} does not exist");
                }

                if (setting.Duration < 1)
                {
                    throw new InvalidDataException($"The storyboard {setting.Name} of item {index} does not exist");
                }

                items.Add(new PlayListItem(storyboard, setting.Duration));
            }

            return new PlayList(settings.Name, items.ToArray());
        }
    }
}
