using SharpYaml.Serialization;
using StellaServerLib.Animation.Mapping;

namespace StellaServerLib.Serialization.Mapping
{
    [YamlTag(nameof(PiMapping))]
    internal class PiMappingSettings
    {
        public int PiIndex { get; set; }

        public int Length { get; set; }

        public int StartIndexOnPi { get; set; }

        public bool InverseDirection { get; set; }
    }
}