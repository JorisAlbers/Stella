﻿using System;
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
using Splat;

namespace StellaServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainControlPanel : ReactiveUserControl<MainControlPanelViewModel>
    {
        public MainControlPanel()
        {
            InitializeComponent();
           

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        viewmodel => viewmodel.AnimationsPanelViewModel,
                        view => view.AnimationsPanelHost.ViewModel)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.StatusViewModel,
                        view => view.StatusControlHost.ViewModel)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.SelectedViewModel,
                        view => view.ViewModelViewHost.ViewModel)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewmodel => viewmodel.TransformationViewModel,
                        view => view.TransformationHost.ViewModel)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
