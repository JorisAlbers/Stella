﻿using System.Collections.Generic;

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
                    AppendBackwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi, piMapping.Length);
                }
                else
                {
                    // Forwards
                    AppendForwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi, piMapping.Length);
                }
            }
            else
            {
                // There are multiple sections
                if (piMapping.FirstSectionIsInverted)
                {
                    // Reversed first
                    // Append first item
                    AppendBackwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi, piMapping.SectionStarts[0]);

                    // Iterate sections in the middle
                    bool currentlyForwards = true;
                    for (int i = 1; i < piMapping.SectionStarts.Length; i++)
                    {
                        if (currentlyForwards)
                        {
                            AppendForwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi + piMapping.SectionStarts[i-1], piMapping.SectionStarts[i] - piMapping.SectionStarts[i-1]);
                        }
                        else
                        {
                            AppendForwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi + piMapping.SectionStarts[i - 1], piMapping.SectionStarts[i] - piMapping.SectionStarts[i - 1]);
                        }

                        currentlyForwards = !currentlyForwards;
                    }
                    // Append last item
                    int lastSectionStart = piMapping.SectionStarts[piMapping.SectionStarts.Length - 1];
                    if (currentlyForwards)
                    {
                        AppendForwardsSection(mask,piMapping.PiIndex, piMapping.StartIndexOnPi + piMapping.SectionStarts[piMapping.SectionStarts.Length-1], piMapping.Length - lastSectionStart);
                    }
                    else
                    {
                        AppendBackwardsSection(mask, piMapping.PiIndex, piMapping.StartIndexOnPi + piMapping.SectionStarts[piMapping.SectionStarts.Length - 1], piMapping.Length - lastSectionStart);
                    }

                }
                else
                {
                    // Forward first

                }

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