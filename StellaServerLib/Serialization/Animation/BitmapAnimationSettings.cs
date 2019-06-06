﻿using SharpYaml.Serialization;
using StellaServerLib.Animation;

namespace StellaServerLib.Serialization.Animation
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
