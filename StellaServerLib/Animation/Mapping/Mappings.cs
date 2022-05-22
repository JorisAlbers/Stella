using System.Collections.Generic;

namespace StellaServerLib.Animation.Mapping
{
    public class Mappings
    {
        public List<ClientMapping> ClientMappings { get; }
        public List<RegionMapping> RegionMappings { get; }

        public Mappings(List<ClientMapping> clientMappings, List<RegionMapping> regionMappings)
        {
            ClientMappings = clientMappings;
            RegionMappings = regionMappings;
        }
    }
}