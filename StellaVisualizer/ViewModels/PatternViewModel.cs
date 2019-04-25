using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using StellaVisualizer.Annotations;

namespace StellaVisualizer.ViewModels
{
    public class PatternViewModel : INotifyPropertyChanged
    {
        public byte Red
        {
            get;
            set;
        }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public Brush Color
        {
            get => new SolidColorBrush(System.Windows.Media.Color.FromRgb(Red, Green, Blue));
        }

        public bool IsSelected { get; set; }

        public PatternViewModel(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
