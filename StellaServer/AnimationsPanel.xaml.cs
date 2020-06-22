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

namespace StellaServer
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
                    view => view.BitmapsListBox.ItemsSource
                ).DisposeWith(disposableRegistration);
            });
        }
    }
}
