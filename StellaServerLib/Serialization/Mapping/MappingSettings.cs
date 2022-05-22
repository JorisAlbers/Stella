using System.Collections.Generic;
using SharpYaml.Serialization;

namespace StellaServerLib.Serialization.Mapping
{
    [YamlTag(nameof(MappingSettings))]
    internal class MappingSettings
    {
        public List<RegionMappingSettings> Mappings { get; set; }
    }
}