using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;

namespace StellaServer.Animation
{
    public class AnimationsPanelViewModel : ReactiveObject
    {
        private readonly BitmapRepository _bitmapRepository;

        [Reactive] public string BitmapFolder { get; set; }
        [Reactive] public IEnumerable<BitmapViewModel> Bitmaps { get; private set; }
        
        public AnimationsPanelViewModel(BitmapRepository bitmapRepository)
        {
            _bitmapRepository = bitmapRepository;
            BitmapFolder = bitmapRepository.FolderPath;
            Bitmaps = CreateBitmapViewModels(bitmapRepository);

            // TODO make bitmap folder settable after server started
            /*this.WhenAnyValue(x => x.BitmapFolder)
                .Throttle(TimeSpan.FromMilliseconds(1000))
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Select(ScanFolderForBitmaps)
                .Where(x=> x!=null)
                .Select(CreateBitmapViewModels)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.Bitmaps);*/
        }
        
        private IEnumerable<BitmapViewModel> CreateBitmapViewModels(BitmapRepository bitmapRepository)
        {
            return bitmapRepository.ListAllBitmaps().Select(x => new BitmapViewModel(x, bitmapRepository.Load(x)));
        }
    }
}
