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

        public ReactiveCommand<BitmapViewModel, BitmapViewModel> BitmapSelected { get; set; } =
            ReactiveCommand.Create<BitmapViewModel, BitmapViewModel>((viewmodel) => viewmodel);

        public BitmapSelectionViewModel(BitmapRepository bitmapRepository, BitmapThumbnailRepository thumbnailRepository)
        {
            _thumbnailRepository = thumbnailRepository;
            BitmapFolder = bitmapRepository.FolderPath;
            Bitmaps = CreateBitmapViewModels(bitmapRepository,thumbnailRepository);
        }

        private IEnumerable<BitmapViewModel> CreateBitmapViewModels(BitmapRepository bitmapRepository,BitmapThumbnailRepository thumbnailRepository)
        {
            return bitmapRepository.ListAllBitmaps().Select(x => new BitmapViewModel(x, thumbnailRepository.Load(x)));
        }
    }
}
