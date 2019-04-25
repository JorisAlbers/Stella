using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;
using StellaVisualizer.Annotations;

namespace StellaVisualizer.ViewModels
{
    public class LedStripViewModel : INotifyPropertyChanged
    {
        public int Length { get; }

        public string Name { get; }

        public List<Frame> Animation { get; set; }
        
        public LedStripViewModel(string name, int length)
        {
            Length = length;
            Name = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
