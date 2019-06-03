using SharpYaml.Serialization;
using StellaServer.Animation.Mapping;

namespace StellaServer.Serialization.Mapping
{
    [YamlTag(nameof(PiMapping))]
    internal class PiMappingSettings
    {
        public int PiIndex { get; set; }

        public int Length { get; set; }

        public int StartIndexOnPi { get; set; }

        public int[] SectionStarts { get; set; }

        public bool FirstSectionIsInverted { get; set; }
    }
}