using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows;
using StellaLib.Animation;
using StellaServerLib.Animation;
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

        public RpiViewModel[] RpiViewModels { get; }


        public IAnimator Animator { get; set; }
        private uint _time;
        private int _lastFrameIndex;
        private int _lengthPerSection;
        private int _sections;

        private Timer _playTimer;


        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            int sections = 2;
            int lengthPerSection = 600;
            _sections = sections;

            _lengthPerSection = lengthPerSection;
            RpiViewModels = new RpiViewModel[3];
            RpiViewModels[0] = new RpiViewModel(sections,lengthPerSection);
            RpiViewModels[1] = new RpiViewModel(sections,lengthPerSection);
            RpiViewModels[2] = new RpiViewModel(sections,lengthPerSection);

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
        private long _startAt;

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

            if (_time > _startAt && _time > _startAt + _nextFramePerLedStripViewModel[0].TimeStampRelative)
            {
                for (int i = 0; i < NUMBER_OF_PIS; i++)
                {
                    RpiViewModels[i].DrawFrame(_nextFramePerLedStripViewModel[i]);
                }

                _nextFramePerLedStripViewModel = Animator.GetNextFramePerPi();
            }
        }

        private void NewAnimationWindowViewModel_OnAnimationCreated(object sender, AnimationCreatedEventArgs e)
        {
            // The user has created a new animation
            _time = 0;
            _lastFrameIndex = 0;

            // If the strip length has changed, create new LedStripViewModels.
            if (e.StripLength != _lengthPerSection)
            {
                //

                /*f
                 or (int i = 0; i < LedStripViewModels.Count; i++)
                {
                    LedStripViewModels[i] = new LedStripViewModel(LedStripViewModels[i].Name, e.StripLength);
                }*/
            }

            Animator = e.Animator;
            for (int i = 0; i < RpiViewModels.Length; i++)
            {
                RpiViewModels[i].Clear();
            }

            _startAt = Animator.GetFrameSetMetadata().TimeStamp.Ticks;
            _nextFramePerLedStripViewModel = Animator.GetNextFramePerPi();
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
