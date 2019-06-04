using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpYaml.Serialization;
using StellaServer.Animation;
using StellaServer.Animation.Mapping;
using StellaServer.Serialization.Mapping;

namespace StellaServer.Serialization.Animation
{
    /// <summary>
    /// Loads a storyboard
    /// </summary>
    public class StoryboardLoader : ILoader<Storyboard>
    {
        public Storyboard Load(StreamReader streamReader)
        {
            var settings = new SerializerSettings();
            settings.RegisterAssembly(typeof(Storyboard).Assembly);
            var serializer = new Serializer(settings);
            Storyboard storyboard = serializer.Deserialize<Storyboard>(streamReader);

            return storyboard;
        }
    }
}
