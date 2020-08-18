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
                    vm => vm.BitmapName,
                    view => view.ImageNameTextBlock.Text,
                    disposableRegistration);

                this.OneWayBind(ViewModel,
                    vm => vm.BitmapImageSource,
                    view => view.BitmapImage.Source,
                    disposableRegistration);

                this.BindCommand(ViewModel,
                    vm => vm.SelectImage,
                    view => view.SelectImageButton);

                this.BindCommand(ViewModel,
                    vm => vm.StraightLayout,
                    view => view.StraightButton);

                this.BindCommand(ViewModel,
                    vm => vm.ArrowHeadLayout,
                    view => view.ArrowHeadButton);

                this.BindCommand(ViewModel,
                    vm => vm.InversedArrowHeadLayout,
                    view => view.InversedArrowHeadButton);

                this.BindCommand(ViewModel,
                    vm => vm.Save,
                    view => view.SaveButton);

                this.BindCommand(ViewModel,
                    vm => vm.Start,
                    view => view.StartButton);

            });
        }
    }
}
