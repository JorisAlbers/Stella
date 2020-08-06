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
using StellaServer.Animation;

namespace StellaServer
{
    /// <summary>
    /// Interaction logic for BitmapViewModel.xaml
    /// </summary>
    public partial class BitmapControl : ReactiveUserControl<BitmapViewModel>
    {
        public BitmapControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        vm => vm.Name,
                        v => v.Name.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        vm => vm.Bitmap,
                        v => v.Image.Source)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
