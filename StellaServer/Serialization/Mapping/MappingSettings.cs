using System.Collections.Generic;
using SharpYaml.Serialization;

namespace StellaServer.Serialization.Mapping
{
    [YamlTag(nameof(MappingSettings))]
    internal class MappingSettings
    {
        public List<PiMappingSettings> Mappings { get; set; }
    }
}