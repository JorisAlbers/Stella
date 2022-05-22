using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpYaml.Serialization;
using StellaLib.Serialization;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Serialization.Mapping
{
    /// <summary>
    /// Loads a list of piMappings from file
    /// </summary>
    public class MappingLoader : ILoader<Mappings>
    {
        public Mappings Load(StreamReader streamReader)
        {
            var settings = new SerializerSettings();
            settings.RegisterAssembly(typeof(MappingSettings).Assembly);
            var serializer = new Serializer(settings);
            MappingSettings mappingSettings = serializer.Deserialize<MappingSettings>(streamReader);

           
            if (mappingSettings.Mappings == null || !mappingSettings.Mappings.Any())
            {
                throw new InvalidDataException("Failed to read mapping, there is no region mapping defined.");
            }
            if (mappingSettings.Clients == null || !mappingSettings.Clients.Any())
            {
                throw new InvalidDataException("Failed to read mapping, there is no client mapping defined.");
            }

            // Convert to list of PiMappings
            List<RegionMapping> regionMappings = new List<RegionMapping>();
            foreach (RegionMappingSettings piMapping in mappingSettings.Mappings)
            {
                regionMappings.Add(new RegionMapping(piMapping.PiIndex, piMapping.Length, piMapping.StartIndexOnPi, piMapping.InverseDirection));
            }

            // Convert to 
            var clientMappings = new List<ClientMapping>();
            foreach (ClientMappingSettings clientMappingSetting in mappingSettings.Clients)
            {
                clientMappings.Add(new ClientMapping(clientMappingSetting.Index, clientMappingSetting.Mac));
            }

            return new Mappings(clientMappings,regionMappings);
        }
    }
}
