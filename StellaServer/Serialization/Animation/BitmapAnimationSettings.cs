using System;
using System.Collections.Generic;
using System.Text;
using SharpYaml.Serialization;
using StellaServer.Animation;

namespace StellaServer.Serialization.Animation
{
    [YamlTag(nameof(DrawMethod.Bitmap))]
    public class BitmapAnimationSettings : IAnimationSettings
    {
        public int StartIndex { get; set; }
        public int StripLength { get; set; }
        public int FrameWaitMs { get; set; }
        public string ImagePath { get; set; }
    }
}
