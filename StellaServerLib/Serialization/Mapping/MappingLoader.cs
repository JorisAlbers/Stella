using System.Collections.Generic;
using System.IO;
using SharpYaml.Serialization;
using StellaLib.Serialization;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Serialization.Mapping
{
    /// <summary>
    /// Loads a list of piMappings from file
    /// </summary>
    public class MappingLoader : ILoader<MappingLoader.Mapping>
    {
        public record Mapping(List<PiMaskItem> Mask, int[] StripLengthPerPi, int Rows, int Columns);


        internal List<RegionMapping> LoadInternal(StreamReader reader)
        {
            var settings = new SerializerSettings();
            settings.RegisterAssembly(typeof(MappingSettings).Assembly);
            var serializer = new Serializer(settings);
            MappingSettings mappingSettings = serializer.Deserialize<MappingSettings>(reader);

            // Convert to list of PiMappings
            List<RegionMapping> mappings = new List<RegionMapping>();
            foreach (RegionMappingSettings piMapping in mappingSettings.Mappings)
            {
                mappings.Add(new RegionMapping(piMapping.PiIndex, piMapping.Length, piMapping.StartIndexOnPi, piMapping.InverseDirection));
            }
            return mappings;
        }

        public Mapping Load(StreamReader streamReader)
        {
            var mappings = LoadInternal(streamReader);

            int rows = mappings.Count;
            int _LEDS_PER_TUBE = 120;
            int columns = mappings[0].Length / _LEDS_PER_TUBE;

            // Convert them to a mask
            PiMaskCalculator piMaskCalculator = new PiMaskCalculator(mappings);
            var piMasks =  piMaskCalculator.Calculate(out int[] stripLengthPerPi);

            return new Mapping(piMasks, stripLengthPerPi, rows, columns);


        }
    }
}
