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

        public List<Frame> Animation { get; set; }
        private uint _time;
        private int _lastFrameIndex;


        
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
            _time = 0;
            _lastFrameIndex = 0;
            Animation = e.Animation;
            for (int i = 0; i < LedStripViewModels.Count; i++)
            {
                LedStripViewModels[i].Clear();
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
            throw new NotImplementedException();
        }
        
        private void NextFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            _time += 50;
            int nextFrameIndex = _lastFrameIndex + 1;

            if (Animation.Count <= nextFrameIndex)
            {
                return;
            }

            Frame nextFrame = Animation[nextFrameIndex];
            if (nextFrame.TimeStampRelative > _time)
            {
                return;
            }

            foreach (LedStripViewModel ledStripViewModel in LedStripViewModels)
            {
                ledStripViewModel.DrawFrame(nextFrame);
            }

            _lastFrameIndex = nextFrameIndex;
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
