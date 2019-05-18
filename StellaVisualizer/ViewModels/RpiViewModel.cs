using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;

namespace StellaVisualizer.ViewModels
{
    public class RpiViewModel
    {
        private int _sections;
        private int _lengthPerSection;

        public LedStripViewModel[] LedStripViewModels { get; }

        public RpiViewModel(int sections, int lengthPerSection)
        {
            _sections = sections;
            _lengthPerSection = lengthPerSection;
            LedStripViewModels = new LedStripViewModel[sections];
            for (int i = 0; i < sections; i++)
            {
                LedStripViewModels[i] = new LedStripViewModel(lengthPerSection);
            }
        }

        public void DrawFrame(Frame frame)
        {
            foreach (PixelInstruction pixelInstruction in frame)
            {
                int sectionIndex = (int) (pixelInstruction.Index / _lengthPerSection);
                LedStripViewModels[sectionIndex].DrawPixel((int) (pixelInstruction.Index % _lengthPerSection),  pixelInstruction.Color);
            }
        }

        public void Clear()
        {
            foreach (LedStripViewModel ledStripViewModel in LedStripViewModels)
            {
                ledStripViewModel.Clear();
            }
        }
    }
}
