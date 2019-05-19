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
using StellaVisualizer.ViewModels;
using Frame = System.Windows.Controls.Frame;

namespace StellaVisualizer.Controls
{
    /// <summary>
    /// Interaction logic for LedStrip.xaml
    /// </summary>
    public partial class LedStrip : UserControl
    {
        private LedStripViewModel ViewModel
        {
            get { return DataContext as LedStripViewModel;}
        }

        public LedStrip()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize pixels
            for (int i = 0; i < ViewModel.Length; i++)
            {
                TheGrid.Children.Add(new Rectangle()
                {
                    Fill = Brushes.Black,
                    Height = 3,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch
                });
            }

            // Subscribe to viewmodel
            ViewModel.ClearRequested += ViewModel_OnClearRequested;
            ViewModel.NewFrameRequested += ViewModel_OnNewFrameRequested;
        }

        private void ViewModel_OnNewFrameRequested(object sender, List<PixelInstruction> e)
        {
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < e.Count; i++)
                {
                   
                        System.Drawing.Color color = e[i].Color;
                        SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
                        ((Rectangle) TheGrid.Children[(int) e[i].Index]).Fill = brush;
                }
            });
        }

        private void ViewModel_OnClearRequested(object sender, EventArgs e)
        {
            SolidColorBrush emptyBrush = new SolidColorBrush(Colors.Black);
            for (int i = 0; i < TheGrid.Children.Count; i++)
            {
                ((Rectangle) TheGrid.Children[i]).Fill = emptyBrush;
            }
        }
    }
}
