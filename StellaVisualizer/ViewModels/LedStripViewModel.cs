using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellaVisualizer.ViewModels
{
    public class LedStripViewModel
    {
        public int Length { get; }

        public string Name { get; }

        public LedStripViewModel(string name, int length)
        {
            Length = length;
            Name = name;
        }
    }
}
