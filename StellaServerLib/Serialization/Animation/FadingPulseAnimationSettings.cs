using System.Drawing;
using SharpYaml;
using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation
{
    [YamlTag(nameof(DrawMethod.FadingPulse))]
    public class FadingPulseAnimationSettings : IAnimationSettings
    {
        public int StartIndex { get; set; }
        public int StripLength { get; set; }
        public int FrameWaitMs { get; set; }
        public int FadeSteps { get; set; }

        [YamlMember(nameof(Color))]
        [YamlStyle(YamlStyle.Flow)]
        internal byte[] InternalColor { get; set; }

        [YamlIgnore]
        public Color Color
        {
            get => Color.FromArgb(InternalColor[0], InternalColor[1], InternalColor[2]);
            set => InternalColor = new byte[] {value.R, value.G, value.B};
        }
    }
}
