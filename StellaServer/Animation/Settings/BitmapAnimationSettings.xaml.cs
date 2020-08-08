using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace StellaServer.Animation.Settings
{
    /// <summary>
    /// Interaction logic for BitmapAnimationSettings.xaml
    /// </summary>
    public partial class BitmapAnimationSettingsControl : ReactiveUserControl<BitmapAnimationSettingsViewModel>
    {
        public BitmapAnimationSettingsControl()
        {
            InitializeComponent();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.RelativeStart,
                        view => view.RelativeStartTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.StripLength,
                        view => view.StripLengthTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.StartIndex,
                        view => view.StartIndexTextBlock.Text)
                    .DisposeWith(disposableRegistration);

                // Bitmap specific

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.BitmapName,
                        view => view.ImageNameTextBlock.Text)
                    .DisposeWith(disposableRegistration);


                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Wraps,
                        view => view.WrapsCheckBlock.IsChecked)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewmodel => viewmodel.Bitmap,
                        view => view.ImageControl.Source,
                        x=> BitmapToImageSource(x))
                    .DisposeWith(disposableRegistration);
            });
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
