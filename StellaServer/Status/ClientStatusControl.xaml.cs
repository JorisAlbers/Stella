using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace StellaServer.Status
{
    /// <summary>
    /// Interaction logic for ClientStatusControl.xaml
    /// </summary>
    public partial class ClientStatusControl : ReactiveUserControl<ClientStatusViewModel>
    {
        public ClientStatusControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.IsConnected,
                        view => view.OnlineIndicator.Foreground,
                        x=> x ? Brushes.Green : Brushes.Red)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
