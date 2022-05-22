using System.Collections.Generic;
using SharpYaml.Serialization;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Serialization.Mapping
{
    [YamlTag(nameof(StellaServerLib.Animation.Mapping.Mappings))]
    internal class MappingSettings
    {
        public List<ClientMappingSettings> Clients { get; set; }
        public List<RegionMappingSettings> Mappings { get; set; }
    }

    [YamlTag(nameof(ClientMapping))]
    internal class ClientMappingSettings
    {
        public int Index { get; set; }

        public string Mac { get; set; }
    }
}