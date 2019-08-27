using System.Collections.Generic;

namespace StellaServerLib.Animation.Mapping
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

        public List<PiMaskItem> Calculate(out int[] stripLengthPerPi)
        {
            List<PiMaskItem> mask = new List<PiMaskItem>();
            
            List<int> stripLengthPerPiList = new List<int>();


            foreach (PiMapping piMapping in _piMappings)
            {
                AppendPiMapping(piMapping, mask);
                
                // Add the length to the strip lengths
                while (stripLengthPerPiList.Count < piMapping.PiIndex +1)
                {
                    stripLengthPerPiList.Add(0);
                }
                
                stripLengthPerPiList[piMapping.PiIndex] += piMapping.Length;
            }

            stripLengthPerPi = stripLengthPerPiList.ToArray();
            return mask;
        }

        private void AppendPiMapping(PiMapping piMapping, List<PiMaskItem> mask)
        {
            if (piMapping.InverseDirection)
            {
                AppendBackwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi, piMapping.Length);
            }
            else
            {
                AppendForwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi, piMapping.Length);
            }
        }

        private void AppendForwardsSection(List<PiMaskItem> mask, int piIndex, int startIndexOnPi, int length)
        {
            for (int i = 0; i < length; i++)
            {
                mask.Add(new PiMaskItem(piIndex,startIndexOnPi + i));
            }
        }

        private void AppendBackwardsSection(List<PiMaskItem> mask, int piIndex, int startIndexOnPi, int length)
        {
            for (int i = 0; i < length; i++)
            {
                mask.Add(new PiMaskItem(piIndex, startIndexOnPi + length-1 -i));
            }
        }
    }
}
