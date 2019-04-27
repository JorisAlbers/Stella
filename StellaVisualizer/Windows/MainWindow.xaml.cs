using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using StellaLib.Animation;
using StellaVisualizer.ViewModels;
using StellaVisualizer.Windows;


namespace StellaVisualizer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        private NewAnimationWindow _newAnimationWindow;
        private NewAnimationWindowViewModel _newAnimationWindowViewModel;

        public ObservableCollection<LedStripViewModel> LedStripViewModels { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LedStripViewModels = new ObservableCollection<LedStripViewModel>();
            LedStripViewModels.Add(new LedStripViewModel("RPI_1",30));
            LedStripViewModels.Add(new LedStripViewModel("RPI_2",30));
            LedStripViewModels.Add(new LedStripViewModel("RP1_3",30));

            _newAnimationWindow = new NewAnimationWindow();
            _newAnimationWindowViewModel = new NewAnimationWindowViewModel();
            _newAnimationWindowViewModel.AnimationCreated += NewAnimationWindowViewModel_OnAnimationCreated;
            _newAnimationWindow.DataContext = _newAnimationWindowViewModel;
            _newAnimationWindow.Show();
        }

        private void NewAnimationWindowViewModel_OnAnimationCreated(object sender, AnimationCreatedEventArgs e)
        {
            // The user has created a new animation
            for (int i = 0; i < LedStripViewModels.Count; i++)
            {
                LedStripViewModels[i].SetAnimation(e.Animation);
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PreviousFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (LedStripViewModel ledStripViewModel in LedStripViewModels)
            {
                ledStripViewModel.PreviousFrame();
            }
        }
        
        private void NextFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (LedStripViewModel ledStripViewModel in LedStripViewModels)
            {
                ledStripViewModel.NextFrame();
            }
        }

        private void SetButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<NewAnimationWindow>().Any())
            {
                _newAnimationWindow = new NewAnimationWindow();
                _newAnimationWindow.DataContext = _newAnimationWindowViewModel;
                _newAnimationWindow.Show();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
