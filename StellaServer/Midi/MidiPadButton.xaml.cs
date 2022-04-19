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

namespace StellaServer.Midi
{
    /// <summary>
    /// Interaction logic for MidiPadButton.xaml
    /// </summary>
    public partial class MidiPadButton
    {
        public MidiPadButton()
        {
            InitializeComponent();

            this.WhenActivated((d) =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Controller,
                        view => view.ControllerIndexTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.KeyDown,
                        view => view.PadBorder.Background,
                        x => x ? new SolidColorBrush(Colors.Red) : null)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.Bpm,
                        view => view.BpmTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.Mode,
                        view => view.BpmTextBlock.Visibility,
                        mode => mode == MdiPadMode.BpmMeasurement ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);

                this.BindCommand(ViewModel,
                        vm => vm.StartBpmTransformation,
                        view => view.PlayButton)
                    .DisposeWith(d);
            });
        }
    }
}
