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
using System.Windows.Shapes;
using StellaVisualizer.ViewModels;

namespace StellaVisualizer.Windows
{
    /// <summary>
    /// Interaction logic for NewAnimationWindow.xaml
    /// </summary>
    public partial class NewAnimationWindow : Window
    {
        public NewAnimationWindow()
        {
            InitializeComponent();
        }

        private void AddPatternButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewAnimationWindowViewModel viewModel = DataContext as NewAnimationWindowViewModel;
            viewModel.AddPatternViewModel();
        }

        private void CreateAnimationButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewAnimationWindowViewModel viewModel = DataContext as NewAnimationWindowViewModel;
            viewModel.CreateAnimation();
        }
    }
}
