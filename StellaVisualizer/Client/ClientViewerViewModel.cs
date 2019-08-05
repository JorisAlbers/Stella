using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellaLib.Animation;

namespace StellaTestSuite.Client
{
    public class ClientViewerViewModel: INotifyPropertyChanged
    {
        public int NumberOfPixels { get; private set; }
        public int NumberOfPixelsPerRow { get; private set; }

        public Action<Color[]> FrameReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientViewerViewModel(int numberOfPixels)
        {
            NumberOfPixels = numberOfPixels;
            NumberOfPixelsPerRow = numberOfPixels / 2;
        }

        public void DrawFrame(Color[] frame)
        {
            FrameReceived.Invoke(frame);
        }
    }
}
