using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;

namespace StellaServer.Animation
{
    public class BitmapSelectionViewModel : ReactiveObject
    {
        private readonly BitmapThumbnailRepository _thumbnailRepository;
        [Reactive] public string BitmapFolder { get; set; }
        [Reactive] public IEnumerable<BitmapViewModel> Bitmaps { get; private set; }

        [Reactive] public BitmapViewModel SelectedItem { get; set; }

        public ReactiveCommand<Unit, BitmapViewModel> BitmapSelected { get; set; } 

        public BitmapSelectionViewModel(BitmapRepository bitmapRepository, BitmapThumbnailRepository thumbnailRepository)
        {
            _thumbnailRepository = thumbnailRepository;
            BitmapFolder = bitmapRepository.FolderPath;
            Bitmaps = CreateBitmapViewModels(bitmapRepository,thumbnailRepository);

            var canExecute = this.WhenAny(
                x => x.SelectedItem,
                (item) => item != null);

            BitmapSelected = ReactiveCommand.Create<Unit, BitmapViewModel>((u) => SelectedItem, canExecute);
        }

        private IEnumerable<BitmapViewModel> CreateBitmapViewModels(BitmapRepository bitmapRepository,BitmapThumbnailRepository thumbnailRepository)
        {
            return bitmapRepository.ListAllBitmaps().Select(x => new BitmapViewModel(x.Name, thumbnailRepository.Load(x.Name)));
        }
    }
}
