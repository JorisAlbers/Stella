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

namespace StellaServer.Animation.Settings
{
    /// <summary>
    /// Interaction logic for BitmapAnimationSettings.xaml
    /// </summary>
    public partial class RepeatingSettingsAnimationControl : ReactiveUserControl<RepeatingPatternsAnimationSettingsViewModel>
    {
        public RepeatingSettingsAnimationControl()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.RelativeStart,
                        view => view.RelativeStartTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.StripLength,
                        view => view.StripLengthTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.StartIndex,
                        view => view.StartIndexTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                // animation specific
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Patterns,
                        view => view.PatternsListView.ItemsSource,
                        vmToViewConverterOverride: new SystemDrawingColorToSolidColorBrushConverter())
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
