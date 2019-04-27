using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace StellaVisualizer.ViewModels
{
    public class PixelViewModel : INotifyPropertyChanged
    {
        public Color Color {get;set; }

        public SolidColorBrush SolidColorBrush => new SolidColorBrush(System.Windows.Media.Color.FromRgb(Color.R,Color.G,Color.B));
    

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
