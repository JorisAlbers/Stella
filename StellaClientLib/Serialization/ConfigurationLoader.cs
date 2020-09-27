using System;
using System.Collections.Generic;
using System.IO;
using SharpYaml.Serialization;
using StellaLib.Serialization;

namespace StellaClientLib.Serialization
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

            if(!ValidateConfigurationSettings(configuration, out List<string> errors))
            {
                throw new FormatException($"Failed to load the configuration. Errors that occured:\n {String.Join("\n", errors)}");
            }

            return new Configuration(configuration.Id, configuration.Ip, configuration.Port, configuration.UdpPort, 
                configuration.LedCount, configuration.PwmPin, configuration.DmaChannel, configuration.Brightness);
        }

        private bool ValidateConfigurationSettings(ConfigurationSettings configuration, out List<string> errors)
        {
            errors = new List<string>();
            if (configuration.Id < 0)
            {
                errors.Add($"The Id must be >= 0.");
            }
            if (String.IsNullOrWhiteSpace(configuration.Ip))
            {
                errors.Add($"The Ip must be set.");
            }
            if (configuration.LedCount < 0)
            {
                errors.Add($"The LedCount must be >= 0.");
            }

            return errors.Count == 0;
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

            /// <summary> The UDP port of StellaServer </summary>
            public int UdpPort { get; set; }

            /// <summary> The number of leds available.</summary>
            public int LedCount { get; set; }

            /// <summary> The pin the pwm is outputted from. </summary>
            public int PwmPin { get; set; }

            /// <summary> The dma channel used to generate the pwm signal.</summary>
            public int DmaChannel { get; set; }

            /// <summary> The minimum frame rate allowed. </summary>
            public int MinimumFrameRate { get; set; }

            /// <summary> The overall brightness </summary>
            public byte Brightness { get; set; }
    }
}
