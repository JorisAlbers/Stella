using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using ReactiveUI.Fody.Helpers;
using StellaServerLib;
using StellaServerLib.Serialization.Animation;

namespace StellaServer.Animation.Settings
{
    public class BitmapAnimationSettingsViewModel : AnimationSettingViewModel
    {
        [Reactive] public string BitmapName { get; set; }
        [Reactive] public BitmapImage Bitmap { get; set; }
        [Reactive] public bool Wraps { get; set; }

        public BitmapAnimationSettingsViewModel(BitmapAnimationSettings animationSettings,
            BitmapRepository bitmapRepository) : base(animationSettings)
        {
            BitmapName = animationSettings.ImageName;
            Wraps = animationSettings.Wraps;
            Bitmap = BitmapToImageSource(bitmapRepository.Load(animationSettings.ImageName));
        }


        public BitmapAnimationSettingsViewModel(BitmapAnimationSettings animationSettings, BitmapImage bitmap) : base(
            animationSettings)
        {
            BitmapName = animationSettings.ImageName;
            Wraps = animationSettings.Wraps;
            Bitmap = bitmap;
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