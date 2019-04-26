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

        public List<Frame> Animation { get; private set; }

        private int _currentAnimationIndex = -1;

        public ObservableCollection<PixelViewModel> PixelViewModels { get; set; }
        
        public LedStripViewModel(string name, int length)
        {
            Length = length;
            Name = name;
            PixelViewModels = new ObservableCollection<PixelViewModel>(Enumerable.Repeat(new PixelViewModel(), length));
        }

        public void SetAnimation(List<Frame> animation)
        {
            Animation = animation;
            _currentAnimationIndex = -1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void PreviousFrame()
        {
            if (_currentAnimationIndex == -1)
            {
                return;
            }

            if (--_currentAnimationIndex == -1)
            {
                // Clear
                foreach (PixelViewModel pixelViewModel in PixelViewModels)
                {
                    pixelViewModel.Color = Color.FromArgb(0, 0, 0);
                }

                return;
            }

            if (Animation != null && _currentAnimationIndex < Animation.Count - 1)
            {
                foreach (PixelInstruction instruction in Animation[_currentAnimationIndex])
                {
                    PixelViewModels[(int)instruction.Index].Color = instruction.Color;
                }
            }
        }

        public void NextFrame()
        {
            if (Animation != null && _currentAnimationIndex++ < Animation.Count -1)
            {
                foreach (PixelInstruction instruction in Animation[_currentAnimationIndex])
                {
                    PixelViewModels[(int)instruction.Index].Color = instruction.Color;
                }

            }
        }
    }
}
