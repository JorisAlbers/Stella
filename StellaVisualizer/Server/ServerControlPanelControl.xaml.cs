using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StellaVisualizer.Server
{
    /// <summary>
    /// Interaction logic for ServerControlPanelControl.xaml
    /// </summary>
    public partial class ServerControlPanelControl : UserControl
    {
        public ServerControlPanelControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            ServerControlPanelViewModel viewmodel = DataContext as ServerControlPanelViewModel;

            if (viewmodel.IsPaused)
            {
                PauseButton.Content = "Pause";
                
            }
            else
            {
                PauseButton.Content = "Continue";
            }

            viewmodel.IsPaused = !viewmodel.IsPaused;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ServerControlPanelViewModel viewmodel = DataContext as ServerControlPanelViewModel;
            viewmodel.BpmViewModel.OnNextBeat();

        }
    }
}
