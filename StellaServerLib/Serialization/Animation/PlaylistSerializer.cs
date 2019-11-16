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
            
            // Validate storyboards and convert to PlayListItems
            List<string> errors = new List<string>();
            for (int i = 0; i < settings.StoryboardSettings.Length; i++)
            {
                Storyboard storyboard = null;

                if (settings.StoryboardSettings[i].Duration < 1)
                {
                    errors.Add($"The duration of item {i} must be 0 or more.");
                }
                
                if (settings.StoryboardSettings[i].Name == null)
                {
                    // Validate storyboard settings
                    if (StoryboardSerializer.AnimationsAreValid(settings.StoryboardSettings[i].AnimationSettings, out List<string> storyboardErrors))
                    {
                        storyboard = new Storyboard(){AnimationSettings = settings.StoryboardSettings[i].AnimationSettings, Name = $"{settings.Name} storyboard {i}"};
                    }
                    else
                    {
                        errors.Add($"Failed to create storyboard of item {i}. Errors: {string.Join("\n", storyboardErrors)}");
                    }
                }
                else
                {
                    // Validate that the storyboard exists
                    storyboard = _storyboards.FirstOrDefault(x => x.Name == settings.StoryboardSettings[i].Name);
                    if (storyboard == null)
                    {
                        errors.Add($"The storyboard {settings.StoryboardSettings[i].Name} of item {i} does not exist");
                    }
                }

                items.Add(new PlayListItem(storyboard, settings.StoryboardSettings[i].Duration));
            }

            // Throw if any errors occured
            if (errors.Count > 0)
            {
                throw new FormatException($"Failed to load the playlist. Errors that occured:\n {String.Join("\n",errors)}");
            }
            
            return new PlayList(settings.Name, items.ToArray());
        }
    }
}
