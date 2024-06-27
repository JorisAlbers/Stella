using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using ReactiveUI;

namespace StellaServer
{
    public partial class BpmControl
    {
        public BpmControl()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel,
                        vm => vm.Bpm,
                        v => v.BpmTextBlock.Text, 
                        x=> double.Round(x, 2, MidpointRounding.AwayFromZero).ToString())
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.Interval,
                        v => v.IntervalTextBlock.Text)
                    .DisposeWith(d);
            });
        }

        private void BpmControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.RegisterBeat.Execute().Subscribe().Dispose();
        }

        private int ViewModelToViewConverterFunc(double value)
        {
            return (int)value;
        }
    }
}