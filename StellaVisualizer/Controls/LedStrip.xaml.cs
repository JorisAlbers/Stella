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
using StellaVisualizer.ViewModels;

namespace StellaVisualizer.Controls
{
    /// <summary>
    /// Interaction logic for LedStrip.xaml
    /// </summary>
    public partial class LedStrip : UserControl
    {
        public LedStrip()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LedStripViewModel vm = DataContext as LedStripViewModel;
            UniformGrid.Columns = vm.Length;
            for (int i = 0; i < vm.Length; i++)
            {
                UniformGrid.Children.Add(new Canvas()
                    { Background = i % 2 == 0 ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Blue) });
            }
        }
    }
}
