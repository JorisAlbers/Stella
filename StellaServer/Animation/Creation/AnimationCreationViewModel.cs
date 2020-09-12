using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Animation;

namespace StellaServer.Animation.Creation
{
    public class AnimationCreationViewModel : ReactiveObject
    {
        private readonly BitmapRepository _bitmapRepository;
        private readonly BitmapStoryboardCreator _creator;
        private readonly int _numberOfRows;
        private readonly int _numberOfTubes;
        private BitmapSelectionViewModel _bitmapSelectionViewModel;
        private BitmapSelectionControl _bitmapSelectionControl;

        [Reactive] public string Name { get; set; }

        [Reactive] public BitmapViewModel BitmapViewModel { get; set; }

        [Reactive] public bool IsLayoutStraight { get; set; } = true;
        [Reactive] public bool IsLayoutArrowHead { get; set; }
        [Reactive] public bool IsLayoutDash { get; set; }
        [Reactive] public int Delay { get; set; } = 500;



        public ReactiveCommand<Unit,Unit> SelectImage { get; set; }
        public ReactiveCommand<Unit,Storyboard> Save { get; set; }
        public ReactiveCommand<Unit,Storyboard> Start { get; set; }
        public ReactiveCommand<Unit,Unit> Back { get; set; }


        public AnimationCreationViewModel(BitmapRepository bitmapRepository, BitmapStoryboardCreator creator,BitmapThumbnailRepository thumbnailRepository, int numberOfRows, int numberOfTubes)
        {
            _bitmapRepository = bitmapRepository;
            _creator = creator;
            _numberOfRows = numberOfRows;
            _numberOfTubes = numberOfTubes;

            this.SelectImage = ReactiveCommand.Create(() =>
            {
                if (_bitmapSelectionViewModel == null)
                {
                    _bitmapSelectionViewModel = new BitmapSelectionViewModel(bitmapRepository, thumbnailRepository);
                    _bitmapSelectionViewModel.BitmapSelected.Subscribe(onNext =>
                    {
                        _bitmapSelectionControl?.Close();
                        BitmapViewModel = onNext;
                    });
                }

                _bitmapSelectionControl = new BitmapSelectionControl();
                _bitmapSelectionControl.ViewModel = _bitmapSelectionViewModel;
                _bitmapSelectionControl.ShowDialog();
            });
            
            var canSave = this.WhenAnyValue(
                x => x.Name,
                x => x.BitmapViewModel,
                x=> x.Delay,
                (name, bitmapViewModel, delay) =>
                    !String.IsNullOrWhiteSpace(name) &&
                    bitmapViewModel != null &&
                    DelayIsValid(delay)
            );

            IObservable<bool> canStart = this.WhenAnyValue(x => x.BitmapViewModel, x=> x.Name, ( bitmapViewModel, name) => bitmapViewModel != null);


            this.Save = ReactiveCommand.Create(CreateStoryboard, canSave);

            this.Start = ReactiveCommand.Create(CreateStoryboard, canStart);

            this.Back = ReactiveCommand.Create(() => {});
        }

        private bool DelayIsValid(int delay)
        {
            if (IsLayoutStraight)
            {
                // no delay required
                return true;
            }

            return delay > 0 && delay < int.MaxValue;
        }

        private Storyboard CreateStoryboard()
        {
            string name = Name;

            if (String.IsNullOrWhiteSpace(name))
            {
                name = "Custom animation";
            }
            return _creator.Create(name, BitmapViewModel.Name, GetSelectedLayoutType(), Delay);
        }

        private LayoutType GetSelectedLayoutType()
        {
            if (IsLayoutArrowHead)
                return LayoutType.ArrowHead;
            if (IsLayoutDash)
                return  LayoutType.Dash;
            if (IsLayoutStraight)
                return  LayoutType.Straight;
            throw new ArgumentOutOfRangeException();
        }
    }
}
