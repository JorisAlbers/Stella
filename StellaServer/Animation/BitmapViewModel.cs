using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace StellaServer.Animation
{
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