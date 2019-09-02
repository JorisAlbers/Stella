using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace StellaVisualizer.Client
{
    /// <summary>
    /// Interaction logic for ClientViewerControl.xaml
    /// </summary>
    public partial class ClientViewerControl : UserControl
    {
        private ClientViewerViewModel ViewModel
        {
            get { return DataContext as ClientViewerViewModel; }
        }

        private DispatcherTimer _timer;
        private System.Drawing.Color[] _frame;

        public ClientViewerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            int rectangleHeight = ViewModel.ChildGridColumns == 0 ? 3 : 1;
            int rectangleWidth = ViewModel.ChildGridRows    == 0 ? 3 : 1;

            // Initialize pixels Row 1
            for (int i = 0; i < ViewModel.NumberOfPixelsPerRow; i++)
            {
                Row1.Children.Add(new Rectangle()
                {
                    Fill = Brushes.Black,
                    Height = rectangleHeight,
                    Width = rectangleWidth,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                });
            }
            // Initialize pixels Row 2
            for (int i = 0; i < ViewModel.NumberOfPixelsPerRow; i++)
            {
                Row2.Children.Add(new Rectangle()
                {
                    Fill = Brushes.Black,
                    Height = rectangleHeight,
                    Width = rectangleWidth,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                });
            }

            // Subscribe to the frame received so we can draw in this code behind
            ViewModel.FrameReceived += FrameReceived;
            _timer = new DispatcherTimer(DispatcherPriority.Render);
            _timer.Interval = TimeSpan.FromMilliseconds(10);
            _timer.Tick += TimerOnTick;
            _timer.Start();

        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            System.Drawing.Color[] frame = _frame;

            if (frame == null)
            {
                return;
            }

            _frame = null;

            DrawFrame(frame);
        }

        private void FrameReceived(System.Drawing.Color[] frame)
        {
            _frame = frame;
        }
        
        private void DrawFrame(System.Drawing.Color[] frame)
        {
            // Row 1
            for (int i = 0; i < ViewModel.NumberOfPixelsPerRow; i++)
            {
                System.Drawing.Color color = frame[i];
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                ((Rectangle)Row1.Children[i]).Fill = brush;
            }

            // Row 2
            for (int i = ViewModel.NumberOfPixelsPerRow; i < ViewModel.NumberOfPixelsPerRow * 2; i++)
            {
                System.Drawing.Color color = frame[i];
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                ((Rectangle)Row2.Children[i - ViewModel.NumberOfPixelsPerRow]).Fill = brush;
            }
        }

    }
}
