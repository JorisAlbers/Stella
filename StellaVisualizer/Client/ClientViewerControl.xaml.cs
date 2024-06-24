using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using Rectangle = System.Windows.Shapes.Rectangle;

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
            int rectangleHeight;
            int rectangleWidth;
            if (ViewModel.Orientation == Orientation.Horizontal)
            {
                rectangleWidth = 3;
                rectangleHeight = 1;
                MainGrid.Columns = 1;
            }
            else
            {
                rectangleWidth = 1;
                rectangleHeight = 3;
                MainGrid.Rows = 1;
            }
            
            if (ViewModel.Orientation == Orientation.Vertical)
            {
                for (int i = 0; i < ViewModel.Rows; i++) // row = column, as this is a vertical orientation
                {
                    UniformGrid grid = new UniformGrid();
                    grid.Columns = 1;

                    for (int j = 0; j < ViewModel.NumberOfPixelsPerRow; j++)
                    {
                        grid.Children.Add(new Rectangle()
                        {
                            Fill = Brushes.Black,
                            Height = rectangleHeight,
                            Width = rectangleWidth,
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        });
                    }

                    MainGrid.Children.Add(grid);
                }
            }
            else
            {
                throw new NotImplementedException("Only the vertical orientation is implemented");
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
            if (ViewModel.Orientation == Orientation.Vertical)
            {
                int index = 0;
                for (int i = 0; i < ViewModel.Rows; i++)
                {
                    var childGrid = ((UniformGrid)MainGrid.Children[i]);

                    for (int j = 0; j < ViewModel.NumberOfPixelsPerRow; j++)
                    {
                        System.Drawing.Color color = frame[index++];
                        SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                        ((Rectangle)childGrid.Children[j]).Fill = brush;
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Only the vertical orientation is implemented");
            }
        }

    }
}
