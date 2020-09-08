using System;
using System.Collections.Generic;
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

namespace StellaServer.Animation.Creation
{
    /// <summary>
    /// Interaction logic for AnimationCreationControl.xaml
    /// </summary>
    public partial class AnimationCreationControl : ReactiveUserControl<AnimationCreationViewModel>
    {
        public AnimationCreationControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                    vm => vm.Name,
                    view => view.NameTextBox.Text,
                    disposableRegistration);

                this.OneWayBind(ViewModel,
                    vm => vm.BitmapViewModel,
                    view => view.BitmapViewModelHost.ViewModel,
                    disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.SelectImage,
                    view => view.SelectImageButton);

                this.Bind(ViewModel,
                    vm => vm.IsLayoutStraight,
                    view => view.StraightButton.IsChecked);

                this.Bind(ViewModel,
                    vm => vm.IsLayoutArrowHead,
                    view => view.ArrowHeadButton.IsChecked);

                this.Bind(ViewModel,
                    vm => vm.IsLayoutDash,
                    view => view.DashButton.IsChecked);

                this.BindCommand(ViewModel,
                    vm => vm.Save,
                    view => view.SaveButton);

                this.BindCommand(ViewModel,
                    vm => vm.Start,
                    view => view.StartButton);

                this.BindCommand(ViewModel,
                    vm => vm.Back,
                    view => view.BackButton);

                this.Bind(ViewModel,
                    vm => vm.Delay,
                    view => view.DelayTextBox.Text);

                this.OneWayBind(ViewModel,
                    vm => vm.IsLayoutStraight,
                    view => view.DelayPanel.Visibility,
                    x => x ? Visibility.Collapsed : Visibility.Visible);
            });

            DelayTextBox.KeyDown += DelayTextBox_OnKeyDown;


        }

        private void DelayTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                return;
            }

            // ignore non- digit key presses
            e.Handled = true;
        }
    }
}
