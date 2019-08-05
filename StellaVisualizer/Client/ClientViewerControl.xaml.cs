using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StellaLib.Animation;

namespace StellaTestSuite.Client
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

        public ClientViewerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize pixels Row 1
            for (int i = 0; i < ViewModel.NumberOfPixelsPerRow; i++)
            {
                Row1.Children.Add(new Rectangle()
                {
                    Fill = Brushes.Black,
                    Height = 1,
                    Width = 3,
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
                    Height = 1,
                    Width = 3,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                });
            }

            // Subscribe to the frame received so we can draw in this code behind
            ViewModel.FrameReceived += FrameReceived;
        }

        private void FrameReceived(System.Drawing.Color[] frame)
        {
            Dispatcher.Invoke(() => { DrawFrame(frame); });
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
