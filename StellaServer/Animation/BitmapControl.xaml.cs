using System.Reactive.Disposables;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace StellaServer.Animation
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
                this.Bind<BitmapViewModel, BitmapControl, string, string>(ViewModel,
                        vm => vm.Name,
                        v => v.Name.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind<BitmapViewModel, BitmapControl, BitmapImage, ImageSource>(ViewModel,
                        vm => vm.Bitmap,
                        v => v.Image.Source)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
