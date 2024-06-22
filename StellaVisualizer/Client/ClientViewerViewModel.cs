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

        public int GridColumns { get; set; }
        public int GridRows { get; set; }

        
        public int ChildGridRows { get; set; }
        public int ChildGridColumns { get; set; }

        public Action<Color[]> FrameReceived;

        public event PropertyChangedEventHandler PropertyChanged;

        public ClientViewerViewModel(int numberOfPixels, Orientation orientation, int columns, int rows)
        {
            NumberOfPixels = numberOfPixels;
            NumberOfPixelsPerRow = numberOfPixels / 2;

            GridColumns = columns;
            GridRows = rows;


            if (orientation == Orientation.Horizontal)
            {
                ChildGridColumns = 0;
                ChildGridRows = 1;
            }
            else
            {
                ChildGridColumns = 1;
                ChildGridRows = 0;
            }
        }

        public void DrawFrame(Color[] frame)
        {
            FrameReceived.Invoke(frame);
        }
    }
}
