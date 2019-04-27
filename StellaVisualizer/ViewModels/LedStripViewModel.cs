using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

        public ObservableCollection<PixelViewModel> PixelViewModels { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        public LedStripViewModel(string name, int length)
        {
            Length = length;
            Name = name;
            PixelViewModels = new ObservableCollection<PixelViewModel>();
            for (int i = 0; i < length; i++)
            {
                PixelViewModels.Add(new PixelViewModel());
            }
        }

        
        public void DrawFrame(Frame frame)
        {
            foreach (PixelInstruction pixelInstruction in frame)
            {
                PixelViewModels[(int)pixelInstruction.Index].Color = pixelInstruction.Color;
            }
        }


        public void Clear()
        {
            foreach (PixelViewModel vm in PixelViewModels)
            {
                vm.Color = Color.FromArgb(0, 0, 0);
            }
        }
    }
}
