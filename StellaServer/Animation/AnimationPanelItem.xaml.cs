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

namespace StellaServer.Animation
{
    /// <summary>
    /// Interaction logic for AnimationPanelItem.xaml
    /// </summary>
    public partial class AnimationPanelItem : ReactiveUserControl<AnimationPanelItemViewModel>
    {
        public AnimationPanelItem()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewmodel => viewmodel.StartCommand,
                        view => view.StartButton)
                    .DisposeWith(disposableRegistration);
            });
        }

        private void SendToPad1MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel?.SendToPad.Execute(0).Subscribe().Dispose();
        }

        private void SendToPad2MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel?.SendToPad.Execute(1).Subscribe().Dispose();
        }

        private void SendToPad3MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel?.SendToPad.Execute(2).Subscribe().Dispose();
        }

        private void SendToPad4MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            ViewModel?.SendToPad.Execute(3).Subscribe().Dispose();
        }
    }
}
