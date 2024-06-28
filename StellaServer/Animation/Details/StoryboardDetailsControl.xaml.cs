using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using StellaServerLib;

namespace StellaServer.Animation.Details
{
    /// <summary>
    /// Interaction logic for StoryboardDetailsControl.xaml
    /// </summary>
    public partial class StoryboardDetailsControl : ReactiveUserControl<StoryboardDetailsControlViewModel>
    {
        public StoryboardDetailsControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                
                this.Bind(ViewModel,
                        viewmodel => viewmodel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.AnimationSettings,
                        view => view.AnimationSettingsListView.ItemsSource)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.UserCanSetStartTimes,
                        view => view.ShapeButtonsStackPanel.Visibility,
                        (b)=> b ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.StartAtTheSameTime,
                    v => v.StartAtTheSameTimeButton)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.StartAsArrowHead,
                    v => v.StartAsArrowHeadButton)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.StartAsForwardSlash,
                    v => v.StartAsForwardSlashButton)
                    .DisposeWith(disposableRegistration);


                this.BindCommand(ViewModel,
                    vm => vm.Back,
                    view => view.BackButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
