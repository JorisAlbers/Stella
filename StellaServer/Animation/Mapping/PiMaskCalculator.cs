using System.Collections.Generic;

namespace StellaServer.Animation.Mapping
{
    /// <summary>
    /// Calculates a PiMask, a list of sorted PiMaskItems that map to a pi and the led strip index on that pi.
    /// The index in the list of piMaskItems corresponds to the combined led strip index
    /// </summary>
    public class PiMaskCalculator
    {
        private readonly List<PiMapping> _piMappings;

        /// <summary>
        /// CTOR
        ///  </summary>
        /// <param name="piMappings">The PiMappings must be ordered and start from the combined index 0.</param>
        public PiMaskCalculator(List<PiMapping> piMappings)
        {
            _piMappings = piMappings;
        }

        public List<PiMaskItem> Calculate()
        {
            List<PiMaskItem> mask = new List<PiMaskItem>();

            foreach (PiMapping piMapping in _piMappings)
            {
                AppendPiMapping(piMapping, mask);
            }

            return mask;
        }

        private void AppendPiMapping(PiMapping piMapping, List<PiMaskItem> mask)
        {
            // Check if there is just one section.
            if (piMapping.SectionStarts.Length == 0)
            {
                if (piMapping.FirstSectionIsInverted)
                {
                    // Reversed
                }
                else
                {
                    // Forwards
                    for (int i = 0; i < piMapping.Length; i++)
                    {
                        int pixelIndexOnPi = piMapping.StartIndexOnPi + i;
                        mask.Add(new PiMaskItem(piMapping.PiIndex, pixelIndexOnPi));
                    }
                }
            }
        }
    }
}
