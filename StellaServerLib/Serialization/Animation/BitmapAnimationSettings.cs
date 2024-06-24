using SharpYaml.Serialization;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;

namespace StellaServerLib.Serialization.Animation
{
    [YamlTag(nameof(DrawMethod.Bitmap))]
    public class BitmapAnimationSettings : IAnimationSettings
    {
        public int StartIndex { get; set; }
        public int StripLength { get; set; }
        public int RowIndex { get; set; }
        public bool StretchToCanvas { get; set; }
        public int RelativeStart { get; set; }
        public string ImageName { get; set; }
        public bool Wraps { get; set; }
    }
}
