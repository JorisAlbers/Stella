using System;
using System.ComponentModel;
using System.Drawing;

namespace StellaVisualizer.Client
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
