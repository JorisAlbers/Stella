using System;
using System.Drawing;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using StellaServerLib;

namespace StellaServer
{
    public class BitmapThumbnailRepository
    {
        private IFileSystem _fileSystem;
        private readonly BitmapRepository _bitmapRepository;
        private IDirectoryInfo _directory;
        
        public BitmapThumbnailRepository(IFileSystem fileSystem, string directoryPath, BitmapRepository bitmapRepository)
        {
            if (!fileSystem.Directory.Exists(directoryPath))
            {
                fileSystem.Directory.CreateDirectory(directoryPath);
            }

            _fileSystem = fileSystem;
            _bitmapRepository = bitmapRepository;
            _directory = fileSystem.Directory.CreateDirectory(directoryPath);
        }

        public async void Create()
        {
            Task.Run(() =>
            {
                foreach (BitmapInfo info in _bitmapRepository.ListAllBitmaps())
                {
                    string thumbnailPath = Path.Combine(_directory.FullName, info.Name + ".png");
                    if (_fileSystem.File.Exists(thumbnailPath))
                    {
                        continue;
                    }

                    CreateThumbnail(thumbnailPath, info.Name);
                }
            });
        }

        public Bitmap Load(string name)
        {
            string fullName = $"{name}.png";
            string path = Path.Combine(_directory.FullName, fullName);

            if (!_fileSystem.File.Exists(path))
            {
                throw new Exception($"There is no thumbnail present at path {fullName}");
            }

            return new Bitmap(Image.FromFile(path));
        }

        private void CreateThumbnail(string thumbnailPath, string bitmap)
        {
            var directory = _fileSystem.FileInfo.FromFileName(thumbnailPath).Directory;
            if (!directory.Exists)
            {
                directory.Create();
            }

            var thumbnail = new Bitmap(_bitmapRepository.Load(bitmap), new Size(200, 200));
            thumbnail.Save(thumbnailPath);
        }
    }
}
