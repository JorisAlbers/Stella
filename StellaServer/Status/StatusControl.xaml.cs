﻿using System;
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

namespace StellaServer.Status
{
    /// <summary>
    /// Interaction logic for StatusControl.xaml
    /// </summary>
    public partial class StatusControl : ReactiveUserControl<StatusViewModel>
    {
        public StatusControl()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(ViewModel,
                        viewmodel => viewmodel.CurrentlyPlaying,
                        view => view.CurrentlyPlayingTextBlock.Text)
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
