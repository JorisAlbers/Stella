using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using StellaLib.Animation;
using StellaServer.Animation;
using StellaServer.Animation.Animators;
using StellaServer.Animation.Animators.Fade;
using StellaVisualizer.model.AnimatorSettings;


namespace StellaVisualizer.ViewModels
{
    /// <summary>
    /// Window for a new animation
    /// </summary>
    public class NewAnimationWindowViewModel : INotifyPropertyChanged
    {
        private readonly string _patternSerializationFilePath = $"{Directory.GetCurrentDirectory()}/pattern.xml";

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

        public string ImagePath { get; set; } = Path.Combine(Environment.CurrentDirectory, "image.png");

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

        public ObservableCollection<PatternViewModel> PatternViewModels { get; } = new ObservableCollection<PatternViewModel>();

        public NewAnimationWindowViewModel()
        {
            AddPatternViewModel(255, 0, 0);
            AddPatternViewModel(0, 255,0);
            AddPatternViewModel(0, 0, 255);
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void CreateAnimation()
        {
            // Validate input
            Color[] pattern = GetPatternFromViewModels(); 

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
                case DrawMethod.FadingPulseAnimator:
                    CreatePulseAnimation(StripLength, pattern[0], WaitMS);
                    break;
                case DrawMethod.Bitmap:
                    CreateBitMapAnimation(StripLength, WaitMS, ImagePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void CreateBitMapAnimation(int stripLength, int waitMs, string imagePath)
        {
            if (!File.Exists(imagePath))
            {
                Console.Out.WriteLine($"The image at {imagePath} does not exist.");
                return;
            }

            Bitmap bitmap = new Bitmap(Image.FromFile(imagePath));
            BitmapAnimator animator = new BitmapAnimator(stripLength, waitMs, bitmap);
            List<Frame> frames = animator.Create();
            OnAnimationCreated(frames);
        }

        private void CreatePulseAnimation(int stripLength, Color color, int waitMs)
        {
            FadingPulseAnimator animator = new FadingPulseAnimator(stripLength, waitMs, color, 150, 30);
            List<Frame> frames = animator.Create();

            Random random = new Random();
            for (int i = 0; i < 1000; i++)
            {
                animator.Create(frames, random.Next(0,stripLength), random.Next(0,1000));
            }
            OnAnimationCreated(frames);
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

        public void AddPatternViewModel()
        {
            PatternViewModel lastItem = PatternViewModels.LastOrDefault();

            if (lastItem == null)
            {
                AddPatternViewModel(0, 0, 0);
            }
            else
            {
                AddPatternViewModel(lastItem.Red,lastItem.Green, lastItem.Blue);
            }
        }

        

        private void AddPatternViewModel(byte red, byte green, byte blue)
        {
            PatternViewModel vm = new PatternViewModel(red, green, blue);
            vm.RemoveRequested += PatternViewModel_OnRemoveRequested;
            PatternViewModels.Add(vm);
        }

        private void PatternViewModel_OnRemoveRequested(object sender, EventArgs e)
        {
            PatternViewModel vm = sender as PatternViewModel;
            vm.RemoveRequested -= PatternViewModel_OnRemoveRequested;
            PatternViewModels.Remove(vm);
        }

        private Color[] GetPatternFromViewModels()
        {
            return PatternViewModels.Select(x => Color.FromArgb(x.Red,x.Green,x.Blue)).ToArray();
        }

        public void StoreAnimation()
        {
            throw new NotImplementedException();
        }

        public void StorePattern()
        {
            PatternSettings patternSettings = new PatternSettings(GetPatternFromViewModels());

            using (StreamWriter myWriter = new StreamWriter(_patternSerializationFilePath, false))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(PatternSettings));
                mySerializer.Serialize(myWriter, patternSettings);
            }

        }

        public void LoadPattern()
        {
            if (!File.Exists(_patternSerializationFilePath))
            {
                return;
            }

            PatternSettings settings;

            using (StreamReader reader = new StreamReader(_patternSerializationFilePath))
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(PatternSettings));
                settings = (PatternSettings) mySerializer.Deserialize(reader);
            }

            PatternViewModels.Clear(); // TODO event handler leak
            foreach (Color color in settings.Pattern)
            {
                AddPatternViewModel(color.R, color.G, color.B);
            }
        }
    }
}
