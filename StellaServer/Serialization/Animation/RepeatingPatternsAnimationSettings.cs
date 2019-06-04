using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SharpYaml;
using SharpYaml.Serialization;
using StellaServer.Animation;

namespace StellaServer.Serialization.Animation
{
    [YamlTag(nameof(DrawMethod.RepeatingPattern))]
    public class RepeatingPatternAnimationSettings : IAnimationSettings
    {
        public int StartIndex { get; set; }
        public int StripLength { get; set; }
        public int FrameWaitMs { get; set; }

        [YamlMember(nameof(Patterns))]
        [YamlStyle(YamlStyle.Flow)]
        internal byte[][][] InternalPattern { get; set; }

        [YamlIgnore]
        public Color[][] Patterns
        {
            get
            {
                Color[][] pattern = new Color[InternalPattern.Length][];
                for (int i = 0; i < InternalPattern.Length; i++)
                {
                    pattern[i] = new Color[InternalPattern[i].Length];
                    for (int j = 0; j < InternalPattern[i].Length; j++)
                    {
                        pattern[i][j] = Color.FromArgb(InternalPattern[i][j][0], InternalPattern[i][j][1], InternalPattern[i][j][2]);
                    }
                }

                return pattern;

            }
            set
            {
                InternalPattern = new byte[value.Length][][];
                for (int i = 0; i < value.Length; i++)
                {
                    InternalPattern[i] = new byte[value[i].Length][];
                    for (int j = 0; j < value[i].Length; j++)
                    {
                        InternalPattern[i][j] = new byte[] { value[i][j].R, value[i][j].G, value[i][j].B };
                    }
                }
            }
        }

    }
}
