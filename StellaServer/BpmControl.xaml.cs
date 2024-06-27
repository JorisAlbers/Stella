using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
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

                this.BindCommand(ViewModel,
                    vm => vm.Reset,
                    v => v.ResetButton).DisposeWith(d);

                bool toggle = false;
                this.WhenAnyObservable(x => x.ViewModel.NextBeatObservable)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe((x) =>
                {
                    if (toggle)
                    {
                        toggle = false;
                        this.BeatIndicatorRectangle.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        toggle = true;
                        this.BeatIndicatorRectangle.Visibility = Visibility.Collapsed;
                    }
                });

                this.WhenAnyObservable(x => x.ViewModel.Reset).Subscribe(x =>
                {
                    this.BeatIndicatorRectangle.Visibility = Visibility.Collapsed;
                });


                // selection mode
                this.Bind(ViewModel,
                    vm => vm.BpmTransformationMode,
                    v => v.BrightnessModeRadioButton.IsChecked,
                    mode => mode == BpmTransformationMode.Reduce_Brightness,
                    isChecked => BpmTransformationMode.Reduce_Brightness
                    );

                this.Bind(ViewModel,
                    vm => vm.BpmTransformationMode,
                    v => v.RedModeRadioButton.IsChecked,
                    mode => mode == BpmTransformationMode.Reduce_Red,
                    isChecked => BpmTransformationMode.Reduce_Red
                );



            });
        }

        private void BpmControl_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.RegisterBeat.Execute().Subscribe().Dispose();
        }
    }
}