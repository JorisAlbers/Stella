using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using StellaServerLib.Animation;
using StellaServerLib.Serialization.Animation;


namespace StellaServerLib.VideoMapping
{
    public class VideoMappingStoryBoardCreator
    {
        private readonly string _videoRepository;
        private readonly BitmapRepository _bitmapRepository;
        private readonly int _rows;
        private readonly VideoConverter _converter;


        public VideoMappingStoryBoardCreator(string videoRepository, BitmapRepository bitmapRepository, int rows, int columns, int ledsPerColumn)
        {
            _videoRepository = videoRepository;
            _bitmapRepository = bitmapRepository;
            _rows = rows;
            _converter = new VideoConverter(bitmapRepository, rows, columns * ledsPerColumn);

        }

        /// <summary>
        /// Appends the bitmap storyboards to the list of storyboards
        /// </summary>
        /// <param name="storyboards"></param>
        public List<Storyboard> Create()
        {
            string videoMappingFolder = "vm";
            var storyBoards = new List<Storyboard>();

            FileInfo[] videos = new DirectoryInfo(_videoRepository).EnumerateFiles("*.mp4").ToArray();
            Parallel.ForEach(videos, ((video) =>
            {
                string videoFileName = video.Name.Remove(video.Name.LastIndexOf('.'));

                string videoFilePrefix = Path.Combine(videoMappingFolder, videoFileName, videoFileName);

                var storyboard = CreateStoryBoard(videoFilePrefix, videoFileName, _rows);

                // Check if we already mapped this video
                if (!_bitmapRepository.BitmapExists(videoFilePrefix + "_row0"))
                {
                    _converter.Convert(video, videoFilePrefix);
                }

                storyBoards.Add(storyboard);
            }));
            return storyBoards;
        }

        private Storyboard CreateStoryBoard(string videoFilePrefix, string videoFileName, int rows)
        {
            Storyboard storyboard = new Storyboard();
            storyboard.Name = videoFileName;
            storyboard.AnimationSettings = new IAnimationSettings[rows];
            for (int i = 0; i < rows; i++)
            {
                storyboard.AnimationSettings[i] = new BitmapAnimationSettings
                {
                    ImageName = @$"{videoFilePrefix}_row{i}",
                    RowIndex = i,
                };
            }

            return storyboard;
        }
    }
}
