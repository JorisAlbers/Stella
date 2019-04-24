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
    /// Interaction logic for PatternControl.xaml
    /// </summary>
    public partial class PatternControl : UserControl
    {

        public PatternControl()
        {
            InitializeComponent();
        }

        private void Grid_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorSelectionPopUp.IsOpen = true;
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            ColorSelectionPopUp.IsOpen = false;
        }
    }
}
