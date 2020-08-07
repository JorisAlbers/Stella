using System.Reactive.Disposables;
using ReactiveUI;

namespace StellaServer.Animation
{
    /// <summary>
    /// Interaction logic for AnimationsPanel.xaml
    /// </summary>
    public partial class BitmapSelectionControl : ReactiveUserControl<BitmapSelectionViewModel>
    {
        public BitmapSelectionControl()
        {
            InitializeComponent();
            
            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        viewmodel => viewmodel.BitmapFolder,
                        view => view.BitmapFolderTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.Bitmaps,
                    view => view.BitmapsPanel.ItemsSource
                ).DisposeWith(disposableRegistration);
            });
        }
    }
}
