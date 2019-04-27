using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Animators;


namespace StellaVisualizer.ViewModels
{
    /// <summary>
    /// Window for a new animation
    /// </summary>
    public class NewAnimationWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The draw methods available
        /// </summary>
        public string[] DrawMethods { get; } = Enum.GetNames(typeof(DrawMethod));

        /// <summary>
        /// The selected draw method
        /// </summary>
        public string SelectedDrawMethod { get; set; } = Enum.GetName(typeof(DrawMethod), DrawMethod.Unknown);

        /// <summary>
        /// The number of milliseconds each frame will be visible for.
        /// </summary>
        public int WaitMS { get; set; } = 100;

        public int StripLength { get; set; } = 300;

        /// <summary>
        /// Fired when the user has created a new animation
        /// </summary>
        public EventHandler<AnimationCreatedEventArgs> AnimationCreated;

        private PatternViewModel _previouslySelectedPatternViewModel;

        private PatternViewModel _selectedPatternViewModel;
        public PatternViewModel SelectedPatternViewModel
        {
            get => _selectedPatternViewModel;
            set
            {
                if (_previouslySelectedPatternViewModel != null)
                {
                    _previouslySelectedPatternViewModel.IsSelected = false;
                }

                _selectedPatternViewModel = value;
                _previouslySelectedPatternViewModel = value;
            }
        }

        public ObservableCollection<PatternViewModel> PatternViewModels { get; } = new ObservableCollection<PatternViewModel>
        {
            new PatternViewModel(255,0,0),
            new PatternViewModel(0,255,0),
            new PatternViewModel(0,0,255)
        };


        public NewAnimationWindowViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void CreateAnimation()
        {
            // Validate input
            Color[] pattern = PatternViewModels.Select(x => Color.FromArgb(x.Red,x.Green,x.Green)).ToArray();

            if (pattern.Length == 0)
            {
                Console.Out.WriteLine("The pattern length must be > 0");
                return;
            }

            if (WaitMS < 11)
            {
                Console.Out.WriteLine("The waitMS must be larger than 10 ");
                return;
            }

            // Create List of Frames
            DrawMethod method = (DrawMethod)Enum.Parse(typeof(DrawMethod), SelectedDrawMethod);
            switch (method)
            {
                case DrawMethod.Unknown:
                    Console.Out.WriteLine($"Method can't be set to unknown");
                    return;
                case DrawMethod.SlidingPattern:
                    CreateSlidingPatternAnimation(StripLength,pattern, WaitMS);
                    break;
                case DrawMethod.RepeatingPattern:
                    break;
                case DrawMethod.MovingPattern:
                    CreateMovingPatternAnimation(StripLength,pattern, WaitMS);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }



        private void CreateSlidingPatternAnimation(int stripLength, Color[] pattern, int waitMs)
        {
            SlidingPatternAnimator animator = new SlidingPatternAnimator(stripLength,waitMs, pattern);
            List<Frame> frames = animator.Create();
            OnAnimationCreated(frames);
        }

        private void CreateMovingPatternAnimation(int stripLength, Color[] pattern, int waitMS)
        {
            MovingPatternAnimator animator = new MovingPatternAnimator(stripLength,waitMS,pattern);
            List<Frame> frames = animator.Create();
            OnAnimationCreated(frames);
        }

        private void OnAnimationCreated(List<Frame> animation)
        {
            EventHandler<AnimationCreatedEventArgs> handler = AnimationCreated;
            if (handler != null)
            {
                handler.Invoke(this,new AnimationCreatedEventArgs(animation, StripLength));
            }
        }
    }
}
