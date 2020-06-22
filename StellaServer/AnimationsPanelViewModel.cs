using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer
{
    public class AnimationsPanelViewModel : ReactiveObject
    {
        [Reactive] public string BitmapFolder { get; set; }

        public extern IEnumerable<string> Bitmaps { [ObservableAsProperty] get; }


        public AnimationsPanelViewModel(string bitmapFolder)
        {
            BitmapFolder = bitmapFolder;

            this.WhenAnyValue(x => x.BitmapFolder)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Select(ScanFolderForBitmaps)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.Bitmaps);
        }

        private IEnumerable<string> ScanFolderForBitmaps(string folderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            if (!directoryInfo.Exists)
            {
                Console.Beep();
                return null;
            }

            return directoryInfo.EnumerateFiles("*.png").Select(x => x.Name).ToArray();
        }
    }
}
