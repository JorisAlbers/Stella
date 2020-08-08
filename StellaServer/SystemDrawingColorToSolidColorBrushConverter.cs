using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ReactiveUI;
using Color = System.Drawing.Color;

namespace StellaServer
{
    public class SystemDrawingColorToSolidColorBrushConverter : IBindingTypeConverter
    {



        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(System.Drawing.Color))
            {
                return 1;
            }

            if (fromType == typeof(System.Drawing.Color[]))
            {
                return 1;
            }

            if (fromType == typeof(System.Drawing.Color[][]))
            {
                return 1;
            }

            return 0;
        }

        public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
        {
            if (@from is Color color)
            {
                result = Convert(color);
                return true;
            }

            if (@from is Color[] colorArray)
            {
                result = colorArray.Select(Convert).ToArray();
                return true;

            }

            if (@from is Color[][] colorArrayArray)
            {
                result = colorArrayArray.Select(x=> x.Select(Convert).ToArray()).ToArray();
                return true;

            }

            result = null;
            return false;
        }

        private SolidColorBrush Convert(Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
