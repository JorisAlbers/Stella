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
        public ReactiveCommand<Unit,Unit> Start { get; set; }


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

            this.Save = ReactiveCommand.Create(() =>
            {
                LayoutType layoutType = LayoutType.Unknown;
                if (IsLayoutArrowHead)
                    layoutType = LayoutType.ArrowHead;
                if (IsLayoutInversedArrowHead)
                    layoutType = LayoutType.InversedArrowHead;
                if (IsLayoutStraight)
                    layoutType = LayoutType.Straight;

                return _creator.Create(Name, BitmapViewModel.Name, layoutType);
            }, 
                canSave);
        }

    }
}
