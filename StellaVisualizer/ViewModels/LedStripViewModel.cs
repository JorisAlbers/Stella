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
        public PixelViewModel[] PixelViewModels { get; }

        public event PropertyChangedEventHandler PropertyChanged;


        public LedStripViewModel(int length)
        {
            PixelViewModels = new PixelViewModel[length];
            for (int i = 0; i < length; i++)
            {
                PixelViewModels[i] = new PixelViewModel();
            }
        }

        public void Clear()
        {
            foreach (PixelViewModel vm in PixelViewModels)
            {
                vm.Color = Color.FromArgb(0, 0, 0);
            }
        }

        public void DrawPixel(int index, Color pixelInstructionColor)
        {
            PixelViewModels[index].Color = pixelInstructionColor;
        }
    }
}
