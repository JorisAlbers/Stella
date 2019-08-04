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
        }

        /// <summary>
        /// Draw a frame. Should be called from the main thread.
        /// </summary>
        /// <param name="frame"></param>
        public void DrawFrame(List<PixelInstruction> frame)
        {
            for (int i = 0; i < frame.Count; i++)
            {
                System.Drawing.Color color = frame[i].Color;
                SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                if (frame[i].Index < ViewModel.NumberOfPixelsPerRow)
                {
                    // Draw on row 1
                    ((Rectangle)Row1.Children[(int)frame[i].Index]).Fill = brush;
                }
                else
                {
                    // Draw on row 2
                    ((Rectangle)Row1.Children[(int)frame[i].Index - ViewModel.NumberOfPixelsPerRow]).Fill = brush;
                }
            }
        }
    }
}
