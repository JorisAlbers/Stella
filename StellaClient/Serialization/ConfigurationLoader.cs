using System.IO;
using SharpYaml.Serialization;
using StellaLib.Serialization;

namespace StellaClient.Serialization
{
    /// <summary>
    /// Load a configuration. Must be in yaml format.
    /// </summary>
    public class ConfigurationLoader : ILoader<Configuration>
    {
        public Configuration Load(StreamReader streamReader)
        {
            var settings = new SerializerSettings();
            settings.RegisterAssembly(typeof(ConfigurationSettings).Assembly);
            var serializer = new Serializer(settings);
            ConfigurationSettings configuration = serializer.Deserialize<ConfigurationSettings>(streamReader);
            
            return new Configuration(configuration.Id, configuration.Ip, configuration.Port,
                configuration.LedCount, configuration.PwmPin, configuration.DmaChannel);
        }
    }

    [YamlTag(nameof(Configuration))]
    internal class ConfigurationSettings
    {
            /// <summary> The ID of this StellaClient. </summary>
            public int Id { get; set; }

            /// <summary> The ip address of stellaServer. </summary>
            public string Ip { get; set; }

            /// <summary> The port of StellaServer.</summary>
            public int Port { get; set; }

            /// <summary> The number of leds available.</summary>
            public int LedCount { get; set; }

            /// <summary> The pin the pwm is outputted from. </summary>
            public int PwmPin { get; set; }

            /// <summary> The dma channel used to generate the pwm signal.</summary>
            public int DmaChannel { get; set; }

    }
}
