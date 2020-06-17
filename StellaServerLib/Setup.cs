using System;
using System.Collections.Generic;
using System.IO;
using StellaServerLib.Animation.Mapping;
using StellaServerLib.Serialization.Mapping;

namespace StellaServerLib
{
    /// <summary>
    /// Defines the current setup
    /// </summary>
    public class Setup
    {
        /// <summary>
        /// The mask to map the combined index to a pi and the led index on that pi
        /// </summary>
        public List<PiMaskItem> Mask { get; }

        /// <summary>
        /// The strip length per pi
        /// </summary>
        public int[] StripLengthPerPi { get; }

        /// <summary>
        /// The number of pi's
        /// </summary>
        public int NumberOfPis => StripLengthPerPi.Length;
        
        /// <summary>
        /// The number of rows
        /// </summary>
        public int Rows { get; }


        public Setup(List<PiMaskItem> mask, int[] stripLengthPerPi, int rows)
        {
            Mask = mask;
            StripLengthPerPi = stripLengthPerPi;
            Rows = rows;
        }

        public static Setup Create(string mappingFilePath, int rows)
        {
            try
            {
                // Read the piMappings from file
                MappingLoader mappingLoader = new MappingLoader();
                List<PiMapping> piMappings = mappingLoader.Load(new StreamReader(mappingFilePath));

                // Convert them to a mask
                PiMaskCalculator piMaskCalculator = new PiMaskCalculator(piMappings);
                List<PiMaskItem> mask = piMaskCalculator.Calculate(out int[] stripLengthPerPi);

                return new Setup(mask,stripLengthPerPi, rows);

            }
            catch (Exception e)
            {
                throw new Exception("Failed to load mask.", e);
            }
        }
    }
}
