using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using ReactiveUI;
using ListViewItem = System.Windows.Controls.ListViewItem;

namespace StellaServer.Animation
{
    /// <summary>
    /// Interaction logic for AnimationsPanel.xaml
    /// </summary>
    public partial class BitmapSelectionControl : ReactiveWindow<BitmapSelectionViewModel>
    {
        public BitmapSelectionControl()
        {
            InitializeComponent();
            
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.BitmapFolder,
                        view => view.BitmapFolderTextBox.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                    viewmodel => viewmodel.Bitmaps,
                    view => view.BitmapsPanel.ItemsSource
                ).DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                    viewmodel => viewmodel.SelectedItem,
                    view => view.BitmapsPanel.SelectedItem
                ).DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                    viewmodel => viewmodel.BitmapSelected,
                    view=> view.OkButton);
                
            });
        }
    }
}
