using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using StellaLib.Animation;
using StellaServerLib.Animation;
using StellaServerLib.Animation.Drawing;
using StellaServerLib.Animation.Drawing.Fade;
using StellaServerLib.Animation.FrameProviding;
using StellaServerLib.Animation.Mapping;
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

        public int StripLength { get; set; } = 1200;
        public int LengthPerSection { get; set; } = 600;

        private string _imageDir;

        public string ImageDir
        {
            get { return _imageDir;}
            set
            {
                _imageDir = value;
                try
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(value);
                    Images = directoryInfo.GetFiles().Select(x => x.FullName).ToArray();
                }
                catch (Exception e)
                {
                    ;
                }
                
            }
        }

        public string[] Images { get; set; }

        public string SelectedImage { get; set; }

        public bool ImageFor3600Pixels { get; set; }

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

            // Get FrameProvider
            IFrameProvider frameProvider = null;
            DrawMethod method = (DrawMethod)Enum.Parse(typeof(DrawMethod), SelectedDrawMethod);
            AnimationTransformation[] animationTransformations = null;
            switch (method)
            {
                case DrawMethod.Unknown:
                    Console.Out.WriteLine($"Method can't be set to unknown");
                    return;
                case DrawMethod.SlidingPattern:
                    animationTransformations = new AnimationTransformation[] { new AnimationTransformation(WaitMS), };
                    frameProvider = new FrameProvider(new SlidingPatternDrawer(0,StripLength, new AnimationTransformation(WaitMS), pattern), animationTransformations[0]);
                    break;
                case DrawMethod.RepeatingPattern:
                    break;
                case DrawMethod.MovingPattern:
                    animationTransformations = new AnimationTransformation[]
                    {
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                    };

                    IFrameProvider frameProvider1 = new FrameProvider(new MovingPatternDrawer(0, LengthPerSection, animationTransformations[0], new Color[] { Color.Red }), animationTransformations[0]);
                    IFrameProvider frameProvider2 = new FrameProvider(new MovingPatternDrawer(LengthPerSection, LengthPerSection * 2, animationTransformations[1], new Color[] { Color.Green }), animationTransformations[1]);
                    IFrameProvider frameProvider3 = new FrameProvider(new MovingPatternDrawer(LengthPerSection * 3 + LengthPerSection / 2, LengthPerSection * 5, animationTransformations[2], new Color[] { Color.Blue }), animationTransformations[0]);
                    
                    frameProvider = new CombinedFrameProvider(new IFrameProvider[]{ frameProvider1, frameProvider2, frameProvider3 }, new int[]{0,0,0} );
                    break;
                case DrawMethod.RandomFade:
                    animationTransformations = new AnimationTransformation[] { new AnimationTransformation(WaitMS), };
                    frameProvider = new FrameProvider(new RandomFadeDrawer(0,StripLength, animationTransformations[0], pattern, 5),animationTransformations[0]);
                    break;
                case DrawMethod.FadingPulse:
                    animationTransformations = new AnimationTransformation[] { new AnimationTransformation(WaitMS), };
                    frameProvider = new FrameProvider(new FadingPulseDrawer(0,StripLength, animationTransformations[0], pattern[0], 30), animationTransformations[0]);
                    break;
                case DrawMethod.Bitmap:

                    if (SelectedImage == null || !File.Exists(SelectedImage))
                    {
                        Console.Out.WriteLine($"The image at {ImageDir} does not exist.");
                        return;
                    }

                    if (ImageFor3600Pixels)
                    {
                        animationTransformations = new AnimationTransformation[]
                        {
                            new AnimationTransformation(50),
                        };

                        frameProvider = new FrameProvider(new BitmapDrawer(0, 3600, false, new Bitmap(Image.FromFile(SelectedImage))), animationTransformations[0]);
                        break;
                    }

                    animationTransformations = new AnimationTransformation[]
                    {
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                    };


                    IFrameProvider frameProvider11 = new FrameProvider(new BitmapDrawer(0,                    LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[0]);
                    IFrameProvider frameProvider22 = new FrameProvider(new BitmapDrawer(LengthPerSection * 1, LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[1]);
                    IFrameProvider frameProvider33 = new FrameProvider(new BitmapDrawer(LengthPerSection * 2, LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[2]);
                    IFrameProvider frameProvider44 = new FrameProvider(new BitmapDrawer(LengthPerSection * 3, LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[3]);
                    IFrameProvider frameProvider55 = new FrameProvider(new BitmapDrawer(LengthPerSection * 4, LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[4]);
                    IFrameProvider frameProvider66 = new FrameProvider(new BitmapDrawer(LengthPerSection * 5, LengthPerSection,  false, new Bitmap(Image.FromFile(SelectedImage))),animationTransformations[5]);

                    //drawer = new SectionDrawer(new IDrawer[] { drawer11, drawer22, drawer33, drawer44, drawer55, drawer66 }, new int[] { 0, 1000, 2000, 3000, 4000, 5000 });
                    frameProvider = new CombinedFrameProvider(new IFrameProvider[] { frameProvider11, frameProvider22, frameProvider33, frameProvider44, frameProvider55, frameProvider66 }, new int[] { 0, 0, 0, 0, 0, 0 });

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PiMaskCalculator piMaskCalculator = new PiMaskCalculator(new List<PiMapping>
            {
                new PiMapping(0,StripLength,0,new int[0], false),
                new PiMapping(1,StripLength,0,new int[0], false),
                new PiMapping(2,StripLength,0,new int[0], false),
            });
            List<PiMaskItem> piMaskItems = piMaskCalculator.Calculate(out int[] stripLengthPerPi);

            IAnimator animator = new Animator(frameProvider, stripLengthPerPi, piMaskItems,animationTransformations);
            OnAnimationCreated(animator);
        }
        

        private void OnAnimationCreated(IAnimator animator)
        {
            EventHandler<AnimationCreatedEventArgs> handler = AnimationCreated;
            if (handler != null)
            {
                handler.Invoke(this,new AnimationCreatedEventArgs(animator, StripLength));
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
