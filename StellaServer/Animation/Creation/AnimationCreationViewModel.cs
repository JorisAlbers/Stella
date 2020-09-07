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

        [Reactive] public string Name { get; set; }

        [Reactive] public BitmapViewModel BitmapViewModel { get; set; }

        [Reactive] public bool IsLayoutStraight { get; set; } = true;
        [Reactive] public bool IsLayoutArrowHead { get; set; }
        [Reactive] public bool IsLayoutInversedArrowHead { get; set; }


        public ReactiveCommand<Unit,Unit> SelectImage { get; set; }
        public ReactiveCommand<Unit,Storyboard> Save { get; set; }
        public ReactiveCommand<Unit,Storyboard> Start { get; set; }


        public AnimationCreationViewModel(BitmapRepository bitmapRepository, BitmapStoryboardCreator creator, int numberOfRows, int numberOfTubes)
        {
            _bitmapRepository = bitmapRepository;
            _creator = creator;
            _numberOfRows = numberOfRows;
            _numberOfTubes = numberOfTubes;

            this.SelectImage = ReactiveCommand.Create(() =>
            {
                if (_bitmapSelectionViewModel == null)
                {
                    _bitmapSelectionViewModel = new BitmapSelectionViewModel(bitmapRepository);
                }

                BitmapSelectionControl bitmapSelectionControl = new BitmapSelectionControl();
                bitmapSelectionControl.ViewModel = _bitmapSelectionViewModel;

                _bitmapSelectionViewModel.BitmapSelected.Subscribe(onNext =>
                {
                    bitmapSelectionControl.Close();
                    BitmapViewModel = onNext;
                });

                bitmapSelectionControl.ShowDialog();
            });
            
            var canSave = this.WhenAnyValue(
                x => x.Name,
                x => x.BitmapViewModel,
                (name, bitmapViewModel) =>
                    !String.IsNullOrWhiteSpace(name) &&
                    bitmapViewModel != null
            );

            IObservable<bool> canStart = this.WhenAnyValue(x => x.BitmapViewModel, x=> x.Name, ( bitmapViewModel, name) => bitmapViewModel != null);


            this.Save = ReactiveCommand.Create(CreateStoryboard, canSave);

            this.Start = ReactiveCommand.Create(CreateStoryboard, canStart);
        }

        private Storyboard CreateStoryboard()
        {
            string name = Name;

            if (String.IsNullOrWhiteSpace(name))
            {
                name = "Custom animation";
            }
            return _creator.Create(name, BitmapViewModel.Name, GetSelectedLayoutType());
        }

        private LayoutType GetSelectedLayoutType()
        {
            if (IsLayoutArrowHead)
                return LayoutType.ArrowHead;
            if (IsLayoutInversedArrowHead)
                return  LayoutType.InversedArrowHead;
            if (IsLayoutStraight)
                return  LayoutType.Straight;
            throw new ArgumentOutOfRangeException();
        }
    }
}
