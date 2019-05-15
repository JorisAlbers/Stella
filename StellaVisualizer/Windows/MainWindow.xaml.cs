using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Drawing.Fade;
using StellaVisualizer.ViewModels;
using StellaVisualizer.Windows;


namespace StellaVisualizer.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window , INotifyPropertyChanged
    {
        private const int NUMBER_OF_PIS = 3;
        private NewAnimationWindow _newAnimationWindow;
        private NewAnimationWindowViewModel _newAnimationWindowViewModel;

        public ObservableCollection<LedStripViewModel> LedStripViewModels { get; set; }

        public IAnimator Animator { get; set; }
        private uint _time;
        private int _lastFrameIndex;

        private Timer _playTimer;


        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            LedStripViewModels = new ObservableCollection<LedStripViewModel>();
            LedStripViewModels.Add(new LedStripViewModel("RPI_1",300));
            LedStripViewModels.Add(new LedStripViewModel("RPI_2",300));
            LedStripViewModels.Add(new LedStripViewModel("RP1_3",300));

            _newAnimationWindow = new NewAnimationWindow();
            _newAnimationWindowViewModel = new NewAnimationWindowViewModel();
            _newAnimationWindowViewModel.AnimationCreated += NewAnimationWindowViewModel_OnAnimationCreated;
            _newAnimationWindow.DataContext = _newAnimationWindowViewModel;
            _newAnimationWindow.Show();

            _playTimer = new Timer(50);
            _playTimer.AutoReset = true;
            _playTimer.Elapsed += PlayTimerOnElapsed;
        }

        private void PlayTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            IncrementTime();
        }

        // TODO race condition
        private Frame[] _nextFramePerLedStripViewModel = null;

        private void IncrementTime()
        {
            _time += 50;

            if (Animator == null)
            {
                _playTimer.Enabled = false;
                return;
            }

            if (_nextFramePerLedStripViewModel == null)
            {
                return;
            }

            for (int i = 0; i < NUMBER_OF_PIS; i++)
            {
                Frame nextFrame = _nextFramePerLedStripViewModel[i];
                if (_time  > nextFrame.TimeStampRelative)
                {
                    nextFrame = Animator.GetNextFrame(i);
                    LedStripViewModels[i].DrawFrame(nextFrame);
                    _nextFramePerLedStripViewModel[i] = nextFrame;
                }
            }
        }

        private void NewAnimationWindowViewModel_OnAnimationCreated(object sender, AnimationCreatedEventArgs e)
        {
            // The user has created a new animation
            _time = 0;
            _lastFrameIndex = 0;

            // If the strip length has changed, create new LedStripViewModels.
            if (e.StripLength != LedStripViewModels[0].Length)
            {
                for (int i = 0; i < LedStripViewModels.Count; i++)
                {
                    LedStripViewModels[i] = new LedStripViewModel(LedStripViewModels[i].Name, e.StripLength);
                }
            }

            Animator = e.Animator;
            for (int i = 0; i < LedStripViewModels.Count; i++)
            {
                LedStripViewModels[i].Clear();
            }

            _nextFramePerLedStripViewModel = new Frame[LedStripViewModels.Count];
            for (int i = 0; i < _nextFramePerLedStripViewModel.Length; i++)
            {
                _nextFramePerLedStripViewModel[i] = Animator.GetNextFrame(i);
            }
        }

        private void PlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            _playTimer.Enabled = true;
        }

        private void PauseButton_OnClick(object sender, RoutedEventArgs e)
        {
            _playTimer.Enabled = false;
        }

        private void PreviousFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void NextFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            IncrementTime();
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
