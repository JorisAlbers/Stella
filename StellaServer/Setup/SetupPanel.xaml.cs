using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace StellaServer.Setup
{
    /// <summary>
    /// Interaction logic for SetupPanel.xaml
    /// </summary>
    public partial class SetupPanel : ReactiveUserControl<SetupPanelViewModel>
    {
        public SetupPanel()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        viewmodel => viewmodel.ServerIp,
                        view => view.IpTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.ServerTcpPort,
                        view => view.TcpPortTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.ServerUdpPort,
                        view => view.UdpPortTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.MappingFilePath,
                        view => view.MappingFilePathTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.BitmapFolder,
                        view => view.BitmapFolderTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.StoryboardFolder,
                        view => view.StoryboardFolderTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.MaximumFrameRate,
                        view => view.MaximumFrameRateTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewmodel => viewmodel.StartCommand,
                        view => view.StartButton)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Errors,
                        view => view.ErrorsListBox.ItemsSource)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Errors,
                        view => view.ErrorCard.Visibility,
                        value => value == null || value.Count == 0 ? Visibility.Collapsed: Visibility.Visible)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
