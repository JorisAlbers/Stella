using System.Reactive.Disposables;
using ReactiveUI;

namespace StellaServer.Animation
{
    /// <summary>
    /// Interaction logic for AnimationsPanel.xaml
    /// </summary>
    public partial class AnimationsPanel : ReactiveUserControl<AnimationsPanelViewModel>
    {
        public AnimationsPanel()
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
