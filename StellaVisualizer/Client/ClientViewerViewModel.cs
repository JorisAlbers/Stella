using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Controls;

namespace StellaVisualizer.Client
{
    public class ClientViewerViewModel: INotifyPropertyChanged
    {
        public int NumberOfPixels { get; private set; }
        public int NumberOfPixelsPerRow { get; private set; }

        public int Columns { get; set; }
        public int Rows { get; set; }
        public Orientation Orientation { get; set; }

        public Action<Color[]> FrameReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientViewerViewModel(int numberOfPixels, Orientation orientation, int pixelsPerColumn, int columns, int rows)
        {
            NumberOfPixels = numberOfPixels;
            NumberOfPixelsPerRow = pixelsPerColumn * columns;

            Columns = columns;
            Rows = rows;

            Orientation = orientation;
        }

        public void DrawFrame(Color[] frame)
        {
            FrameReceived.Invoke(frame);
        }
    }
}
