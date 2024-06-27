using System.Reactive.Disposables;
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
                        v => v.BpmTextBlock.Text)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel,
                        vm => vm.Interval,
                        v => v.IntervalTextBlock.Text)
                    .DisposeWith(d);
            });
        }
    }
}