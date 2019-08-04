using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellaTestSuite.Client
{
    public class ClientViewerViewModel: INotifyPropertyChanged
    {
        public int NumberOfPixels { get; private set; }
        public int NumberOfPixelsPerRow { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientViewerViewModel(int numberOfPixels)
        {
            NumberOfPixels = numberOfPixels;
            NumberOfPixelsPerRow = numberOfPixels / 2;
        }

    }
}
