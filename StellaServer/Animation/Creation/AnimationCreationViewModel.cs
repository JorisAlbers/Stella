using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Windows.Media;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;

namespace StellaServer.Animation.Creation
{
    public class AnimationCreationViewModel : ReactiveObject
    {
        private readonly BitmapRepository _bitmapRepository;
        private readonly int _numberOfRows;
        private readonly int _numberOfTubes;
        private BitmapSelectionViewModel _bitmapSelectionViewModel;

        [Reactive] public string Name { get; set; }

        [Reactive] public BitmapViewModel BitmapViewModel { get; set; }


        public ReactiveCommand<Unit,Unit> SelectImage { get; set; }
        public ReactiveCommand<Unit,Unit> ArrowHeadLayout { get; set; }
        public ReactiveCommand<Unit,Unit> InversedArrowHeadLayout { get; set; }
        public ReactiveCommand<Unit,Unit> StraightLayout { get; set; }
        public ReactiveCommand<Unit,Unit> Save { get; set; }
        public ReactiveCommand<Unit,Unit> Start { get; set; }


        public AnimationCreationViewModel(BitmapRepository bitmapRepository, int numberOfRows, int numberOfTubes)
        {
            _bitmapRepository = bitmapRepository;
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
                //save;
            }, 
                canSave);
        }


    }
}
