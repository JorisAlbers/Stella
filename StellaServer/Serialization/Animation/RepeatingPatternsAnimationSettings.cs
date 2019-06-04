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
        internal byte[][][] InternalPatterns { get; set; }

        [YamlIgnore]
        public Color[][] Patterns
        {
            get
            {
                Color[][] pattern = new Color[InternalPatterns.Length][];
                for (int i = 0; i < InternalPatterns.Length; i++)
                {
                    pattern[i] = new Color[InternalPatterns[i].Length];
                    for (int j = 0; j < InternalPatterns[i].Length; j++)
                    {
                        pattern[i][j] = Color.FromArgb(InternalPatterns[i][j][0], InternalPatterns[i][j][1], InternalPatterns[i][j][2]);
                    }
                }

                return pattern;

            }
            set
            {
                InternalPatterns = new byte[value.Length][][];
                for (int i = 0; i < value.Length; i++)
                {
                    InternalPatterns[i] = new byte[value[i].Length][];
                    for (int j = 0; j < value[i].Length; j++)
                    {
                        InternalPatterns[i][j] = new byte[] { value[i][j].R, value[i][j].G, value[i][j].B };
                    }
                }
            }
        }

    }
}
