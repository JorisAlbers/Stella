﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpYaml.Serialization;
using StellaServer.Animation.Mapping;

namespace StellaServer.Serialization.Mapping
{
    /// <summary>
    /// Loads a list of piMappings from file
    /// </summary>
    public class MappingLoader : ILoader<List<PiMapping>>
    {
        public List<PiMapping> Load(StreamReader streamReader)
        {
            var settings = new SerializerSettings();
            settings.RegisterAssembly(typeof(MappingSettings).Assembly);
            var serializer = new Serializer(settings);
            MappingSettings mappingSettings = serializer.Deserialize<MappingSettings>(streamReader);

            // Convert to list of PiMappings
            List<PiMapping> mappings = new List<PiMapping>();
            foreach (PiMappingSettings piMapping in mappingSettings.Mappings)
            {
                mappings.Add(new PiMapping(piMapping.PiIndex, piMapping.Length, piMapping.StartIndexOnPi, piMapping.SectionStarts, piMapping.FirstSectionIsInverted));
            }

            return mappings;
        }
    }
}