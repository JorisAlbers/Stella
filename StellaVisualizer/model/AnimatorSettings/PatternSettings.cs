using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace StellaVisualizer.model.AnimatorSettings
{
    [XmlRoot]
    public class PatternSettings
    {
        [XmlIgnore]
        public Color[] Pattern { get; set; }

        [XmlArray(nameof(Pattern))]
        public SerializableColor[] SerializableColors {
            get { return  Pattern.Select(x => new SerializableColor{Color = x}).ToArray(); }
            set { Pattern = value.Select(x => x.Color).ToArray(); }
        }

        public PatternSettings(Color[] pattern)
        {
            Pattern = pattern;
        }

        public PatternSettings()
        {
            
        }

        public class SerializableColor
        {

            [XmlIgnore]
            public Color Color { get; set; }

            [XmlElement("BackColor")]
            public int BackColorAsArgb
            {
                get { return Color.ToArgb(); }
                set { Color = Color.FromArgb(value); }
            }


            public SerializableColor()
            {
                
            }
        }
    }
}
