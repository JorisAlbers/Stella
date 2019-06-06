using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using StellaServer.Animation.Mapping;
using StellaServer.Serialization;
using StellaServer.Serialization.Mapping;

namespace StellaServer
{
    public class StellaServer
    {
        private readonly string _mappingFilePath;
        private List<PiMaskItem> _mask;


        public StellaServer(string mappingFilePath)
        {
            _mappingFilePath = mappingFilePath;
        }

        public void Start()
        {
            // Read mapping
            try
            {
                _mask = LoadMask(_mappingFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to load mask.",e);
            }
        }

        private List<PiMaskItem> LoadMask(string mappingFilePath)
        {
            // Read the piMappings from file
            MappingLoader mappingLoader = new MappingLoader();
            List<PiMapping> piMappings = mappingLoader.Load(new StreamReader(mappingFilePath));

            // Convert them to a mask
            PiMaskCalculator piMaskCalculator = new PiMaskCalculator(piMappings);
            return piMaskCalculator.Calculate();
        }
    }
}
