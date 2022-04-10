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

namespace StellaServer.Transformations
{
    /// <summary>
    /// Interaction logic for TransformationControl.xaml
    /// </summary>
    public partial class TransformationControl : ReactiveUserControl<TransformationViewModel>
    {
        public TransformationControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        viewmodel => viewmodel.RedCorrection,
                        view => view.RedCorrectionSlider.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.GreenCorrection,
                        view => view.GreenCorrectionSlider.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.BlueCorrection,
                        view => view.BlueCorrectionSlider.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.BrightnessCorrection,
                        view => view.BrightnessCorrectionSlider.Value)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.TimeUnitsPerFrame,
                        view => view.TimeUnitsPerFrameSlider.Value)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewmodel => viewmodel.Reset,
                    view => view.ResetButton)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    vm => vm.TimeUnitsPerFrame,
                    view => view.TimeUnitsPerFrameTextBlock.Text)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
