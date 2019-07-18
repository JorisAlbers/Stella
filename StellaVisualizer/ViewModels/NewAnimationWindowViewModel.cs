﻿using System;
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

            // Get Drawer
            IDrawer drawer = null;
            DrawMethod method = (DrawMethod)Enum.Parse(typeof(DrawMethod), SelectedDrawMethod);
            AnimationTransformation[] animationTransformations = null;
            switch (method)
            {
                case DrawMethod.Unknown:
                    Console.Out.WriteLine($"Method can't be set to unknown");
                    return;
                case DrawMethod.SlidingPattern:
                    drawer = new SlidingPatternDrawer(0,StripLength, new AnimationTransformation(WaitMS), pattern);
                    animationTransformations = new AnimationTransformation[]{new AnimationTransformation(WaitMS), };
                    break;
                case DrawMethod.RepeatingPattern:
                    break;
                case DrawMethod.MovingPattern:
                    //drawer = new MovingPatternDrawer(0, StripLength, WaitMS, pattern);
                    IDrawer drawer1 = new MovingPatternDrawer(0,LengthPerSection, new AnimationTransformation(WaitMS), new Color[]{Color.Red});
                    IDrawer drawer2 = new MovingPatternDrawer(LengthPerSection,LengthPerSection*2, new AnimationTransformation(WaitMS), new Color[] { Color.Green });
                    IDrawer drawer3 = new MovingPatternDrawer(LengthPerSection * 3 + LengthPerSection /2, LengthPerSection * 5, new AnimationTransformation(WaitMS), new Color[] { Color.Blue });

                    drawer = new SectionDrawer(new IDrawer[]{drawer1,drawer2,drawer3}, new int[]{0,0,0} );
                    animationTransformations = new AnimationTransformation[]
                    {
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                    };


                    break;
                case DrawMethod.RandomFade:
                    drawer = new RandomFadeDrawer(0,StripLength, new AnimationTransformation(WaitMS), pattern, 5);
                    animationTransformations = new AnimationTransformation[] { new AnimationTransformation(WaitMS), };
                    break;
                case DrawMethod.FadingPulse:
                    drawer = new FadingPulseDrawer(0,StripLength, new AnimationTransformation(WaitMS), pattern[0], 30);
                    animationTransformations = new AnimationTransformation[] { new AnimationTransformation(WaitMS), };
                    break;
                case DrawMethod.Bitmap:
                    if (SelectedImage == null || !File.Exists(SelectedImage))
                    {
                        Console.Out.WriteLine($"The image at {ImageDir} does not exist.");
                        return;
                    }

                    if (ImageFor3600Pixels)
                    {
                        drawer = new BitmapDrawer(0, 3600, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                        animationTransformations = new AnimationTransformation[]
                        {
                            new AnimationTransformation(50),
                        };
                        break;
                    }


                    IDrawer drawer11 = new BitmapDrawer(0, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                    IDrawer drawer22 = new BitmapDrawer(LengthPerSection * 1, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                    IDrawer drawer33 = new BitmapDrawer(LengthPerSection * 2, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                    IDrawer drawer44 = new BitmapDrawer(LengthPerSection * 3, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                    IDrawer drawer55 = new BitmapDrawer(LengthPerSection * 4, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));
                    IDrawer drawer66 = new BitmapDrawer(LengthPerSection * 5, LengthPerSection, new AnimationTransformation(WaitMS), false, new Bitmap(Image.FromFile(SelectedImage)));

                    //drawer = new SectionDrawer(new IDrawer[] { drawer11, drawer22, drawer33, drawer44, drawer55, drawer66 }, new int[] { 0, 1000, 2000, 3000, 4000, 5000 });
                    drawer = new SectionDrawer(new IDrawer[] { drawer11, drawer22, drawer33, drawer44, drawer55, drawer66 }, new int[] { 0, 0, 0, 0, 0, 0 });
                    animationTransformations = new AnimationTransformation[]
                    {
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                        new AnimationTransformation(WaitMS),
                    };
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

            IAnimator animator = new Animator(drawer, stripLengthPerPi, piMaskItems,animationTransformations);
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
