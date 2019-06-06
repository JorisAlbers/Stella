﻿using System.Drawing;
using SharpYaml;
using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation
{
    [YamlTag(nameof(DrawMethod.SlidingPattern))]
    public class SlidingPatternAnimationSettings : IAnimationSettings
    {
        public int StartIndex { get; set; }
        public int StripLength { get; set; }
        public int FrameWaitMs { get; set; }

        [YamlMember(nameof(Pattern))]
        [YamlStyle(YamlStyle.Flow)]
        internal byte[][] InternalPattern { get; set; }

        [YamlIgnore]
        public Color[] Pattern
        {
            get
            {
                Color[] pattern = new Color[InternalPattern.Length];
                for (int i = 0; i < InternalPattern.Length; i++)
                {
                    pattern[i] = Color.FromArgb(InternalPattern[i][0], InternalPattern[i][1], InternalPattern[i][2]);
                }

                return pattern;

            }
            set
            {
                InternalPattern = new byte[value.Length][];
                for (int i = 0; i < value.Length; i++)
                {
                    InternalPattern[i] = new byte[] { value[i].R, value[i].G, value[i].B };
                }
            }
        }

    }
}