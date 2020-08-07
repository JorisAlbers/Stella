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

namespace StellaServer.Animation
{
    /// <summary>
    /// Interaction logic for AnimationPanelItem.xaml
    /// </summary>
    public partial class AnimationPanelItem : ReactiveUserControl<AnimationPanelItemViewModel>
    {
        public AnimationPanelItem()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Name,
                        view => view.NameTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewmodel => viewmodel.StartCommand,
                        view => view.StartButton)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
