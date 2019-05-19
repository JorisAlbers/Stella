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
        public int Length { get; set; }

        /// <summary> Fired then the led strip should be cleared </summary>
        public event EventHandler ClearRequested;
        /// <summary> Fired when the led strip should draw a new frame </summary>
        public event EventHandler<List<PixelInstruction>> NewFrameRequested;

        public event PropertyChangedEventHandler PropertyChanged;


        public LedStripViewModel(int length)
        {
            Length = length;
        }

        public void Clear()
        {
            EventHandler eventHandler = ClearRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this,new EventArgs());
            }
        }

        public void SetFrame(List<PixelInstruction> pixelInstructions)
        {
            EventHandler<List<PixelInstruction>> eventHandler = NewFrameRequested;
            if (eventHandler != null)
            {
                eventHandler.Invoke(this, pixelInstructions);
            }
        }
    }
}
