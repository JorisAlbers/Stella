using System.Windows;
using System.Windows.Controls;

namespace StellaVisualizer.Server
{
    /// <summary>
    /// Interaction logic for ServerConfigurationControl.xaml
    /// </summary>
    public partial class ServerConfigurationControl : UserControl
    {
        public ServerConfigurationControl()
        {
            InitializeComponent();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerConfigurationViewModel viewmodel = DataContext as ServerConfigurationViewModel;
            viewmodel.Apply();


        }
    }
}
