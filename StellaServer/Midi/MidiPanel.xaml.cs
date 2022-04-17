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
    /// Interaction logic for MidiPanel.xaml
    /// </summary>
    public partial class MidiPanel
    {
        public MidiPanel()
        {
            InitializeComponent();

            this.WhenActivated((d) =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.Pads,
                    view => view.PadButtonsListView.ItemsSource)
                    .DisposeWith(d);
            });
        }
    }
}
