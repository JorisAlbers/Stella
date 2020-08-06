using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StellaServer.Animation
{
    public class AnimationsPanelViewModel : ReactiveObject
    {

        [Reactive] public string BitmapFolder { get; set; }
        
        public extern IEnumerable<BitmapViewModel> Bitmaps { [ObservableAsProperty] get; }
        
        public AnimationsPanelViewModel(string bitmapFolder)
        {
            BitmapFolder = bitmapFolder;

            this.WhenAnyValue(x => x.BitmapFolder)
                .Throttle(TimeSpan.FromMilliseconds(1000))
                .Select(x => x?.Trim())
                .DistinctUntilChanged()
                .Select(ScanFolderForBitmaps)
                .Where(x=> x!=null)
                .Select(CreateBitmapViewModels)
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.Bitmaps);
        }

        private FileInfo[] ScanFolderForBitmaps(string folderPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
            if (!directoryInfo.Exists)
            {
                //throw new ArgumentException($"folder does not exist");
                Console.Beep();
                return null;
            }

            return directoryInfo.EnumerateFiles("*.png").ToArray();
        }

        private IEnumerable<BitmapViewModel> CreateBitmapViewModels(FileInfo[] bitmapFiles)
        {
            return bitmapFiles.Select(x => new BitmapViewModel(x.Name, new Bitmap(Bitmap.FromFile(x.FullName))));
        }
    }

    public class BitmapViewModel : ReactiveObject
    {
        public string Name { get; }
        public BitmapImage Bitmap { get; }



        public BitmapViewModel(string name, Bitmap bitmap)
        {
            Name = name;
            Bitmap = BitmapToImageSource(bitmap);
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
