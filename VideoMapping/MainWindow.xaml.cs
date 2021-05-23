using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
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

namespace VideoMapping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated((d) =>
            {
                this.Bind(ViewModel, vm => vm.PixelsPerRow, v => v.PixelsPerRowTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.Rows, v => v.RowsTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.StoryboardFolder, v => v.StoryboardFolderTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.OutputFolder, v => v.OutputFolderTextBox.Text).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.InputFolder, v => v.InputFolderTextBox.Text).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.Start, v => v.StartButton).DisposeWith(d);
            });
        }
    }
}
